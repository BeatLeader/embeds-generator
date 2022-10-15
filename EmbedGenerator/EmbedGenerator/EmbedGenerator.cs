using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace EmbedGenerator;

internal class EmbedGenerator {
    #region Constants

    private static readonly Color CoverGradientTint = Color.FromArgb(128, 255, 255, 255);
    private static readonly Color CoverImageTint = Color.FromArgb(255, 140, 140, 140);
    private const int CoverImageBlur = 12;

    private readonly NumberFormatInfo _numberFormatInfo = new() {
        NumberGroupSeparator = "",
        NumberDecimalSeparator = ".",
        NumberDecimalDigits = 2
    };

    #endregion
    
    #region Constructor

    private readonly Image _avatarMask;
    private readonly Image _gradientMask;
    private readonly Image _backgroundImage;
    private readonly Image _coverMask;
    private readonly Image _finalMask;
    private readonly EmbedLayout _layout;
    private readonly Bitmap _fullSizeEmptyBitmap;
    private readonly FontFamily _fontFamily;

    public EmbedGenerator(
        Size size,
        Image avatarMask,
        Image backgroundImage,
        Image gradientMask,
        Image coverMask,
        Image finalMask, 
        FontFamily fontFamily
    ) {
        _fontFamily = fontFamily;
        _layout = new EmbedLayout(size);

        _avatarMask = avatarMask.ResizeIfNecessary(_layout.AvatarRectangle.Size);
        _backgroundImage = backgroundImage.ResizeIfNecessary(_layout.FullRectangle.Size);
        _gradientMask = gradientMask.ResizeIfNecessary(_layout.FullRectangle.Size);
        _coverMask = coverMask.ResizeIfNecessary(_layout.FullRectangle.Size);
        _finalMask = finalMask.ResizeIfNecessary(_layout.FullRectangle.Size);

        _fullSizeEmptyBitmap = new Bitmap(_layout.Width, _layout.Height);
    }

    #endregion

    #region Generate

    public Image Generate(
        string playerName,
        string songName,
        string modifiers,
        string difficulty,
        float accuracy,
        int rank,
        float pp,
        float stars,
        Image coverImage,
        Image avatarImage,
        Image? avatarOverlayImage,
        int overlayHueShift,
        float overlaySaturation,
        Color leftColor,
        Color rightColor,
        Color diffColor
    ) {
        var gradient = GenerateGradient(leftColor, rightColor);
        var cover = GenerateCover(coverImage, gradient);
        var avatar = GenerateAvatar(avatarImage);

        var factory = new ImageFactory()
            .Load(_backgroundImage)
            .OverlayRegion(gradient)
            .OverlayRegion(cover)
            .OverlayRegion(avatar, _layout.AvatarRectangle);

        if (avatarOverlayImage != null) {
            var avatarOverlay = GenerateAvatarOverlay(avatarOverlayImage, overlayHueShift, overlaySaturation);
            factory.OverlayRegion(avatarOverlay, _layout.AvatarOverlayRectangle);
        }
        
        var accuracyText = $"{(accuracy * 100).ToString(_numberFormatInfo)}%";
        var rankText = pp != 0 ? $"#{rank} • {pp.ToString(_numberFormatInfo)}pp" : $"#{rank}";
        var diffText = stars != 0 ? $"{difficulty} {stars.ToString(_numberFormatInfo)}★" : difficulty;

        var graphics = Graphics.FromImage(factory.Image);
        graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        graphics.FitText(playerName, Color.White, _fontFamily, FontStyle.Bold, _layout.PlayerNameRectangle, _layout.MinPlayerNameFontSize);
        graphics.FitText(songName, Color.White, _fontFamily, FontStyle.Bold, _layout.SongNameRectangle, _layout.MinSongNameFontSize);
        graphics.FitText(accuracyText, Color.White, _fontFamily, FontStyle.Bold, _layout.AccTextRectangle);
        graphics.FitText(rankText, Color.White, _fontFamily, FontStyle.Bold, _layout.RankTextRectangle);
        graphics.FitText(modifiers, Color.White, _fontFamily, FontStyle.Bold, _layout.ModifiersTextRectangle);
        graphics.FitText(diffText, diffColor, _fontFamily, FontStyle.Bold, _layout.DiffTextRectangle);
        // DrawDebugInfo(graphics);
        
        factory.MaskRegion(_finalMask);
        return factory.Image;
    }

    #endregion

    #region Debug

    private void DrawDebugInfo(Graphics graphics) {
        var debugPen = new Pen(new SolidBrush(Color.Red), 1f);
        graphics.DrawRectangle(debugPen, _layout.AvatarRectangle);
        graphics.DrawRectangle(debugPen, _layout.SongNameRectangle);
        graphics.DrawRectangle(debugPen, _layout.PlayerNameRectangle);
        graphics.DrawRectangle(debugPen, _layout.AccTextRectangle);
        graphics.DrawRectangle(debugPen, _layout.RankTextRectangle);
        graphics.DrawRectangle(debugPen, _layout.ModifiersTextRectangle);
        graphics.DrawRectangle(debugPen, _layout.DiffTextRectangle);
    }

    #endregion

    #region GenerateAvatar

    private Image GenerateAvatar(Image avatarImage) {
        var factory = new ImageFactory()
            .Load(avatarImage)
            .Resize(new ResizeLayer(_layout.AvatarRectangle.Size, ResizeMode.Stretch))
            .MaskRegion(_avatarMask, _layout.AvatarRectangle.Size);

        return factory.Image;
    }

    #endregion

    #region GenerateAvatarOverlay

    private Image GenerateAvatarOverlay(
        Image avatarOverlayImage,
        int hueShiftDegrees,
        float saturation
    ) {
        return avatarOverlayImage
            .ApplyHsbTransform(hueShiftDegrees, saturation, 0f)
            .ResizeIfNecessary(_layout.AvatarOverlayRectangle.Size);
    }

    #endregion

    #region GenerateGradient

    private Image GenerateGradient(Color leftColor, Color rightColor) {
        var gradientBrush = new LinearGradientBrush(
            new Point(0, _layout.Height),
            new Point(_layout.Width, 0),
            leftColor, rightColor
        );
        
        var factory = new ImageFactory().Load(_fullSizeEmptyBitmap);
        var graphics = Graphics.FromImage(factory.Image);
        graphics.FillRectangle(gradientBrush, _layout.FullRectangle);
        factory.MaskRegion(_gradientMask);

        return factory.Image;
    }

    #endregion

    #region GenerateCover

    private Image GenerateCover(Image coverImage, Image gradient) {
        var fadedGradient = new ImageFactory()
            .Load(gradient)
            .Tint(CoverGradientTint);
            
        var factory = new ImageFactory()
            .Load(coverImage)
            .Resize(new ResizeLayer(_layout.Size, ResizeMode.Crop))
            .OverlayRegion(fadedGradient.Image)
            .Tint(CoverImageTint)
            .GaussianBlur(CoverImageBlur)
            .MaskRegion(_coverMask, _layout.FullRectangle, ResizeMode.Stretch);

        return factory.Image;
    }

    #endregion
}