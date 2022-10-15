using System.Drawing;

namespace EmbedGenerator;

internal class EmbedLayout {
    #region Properties

    public readonly Rectangle FullRectangle;
    public readonly Rectangle AvatarRectangle;
    public readonly Rectangle AvatarOverlayRectangle;
    public readonly Rectangle SongNameRectangle;
    public readonly Rectangle PlayerNameRectangle;
    public readonly Rectangle AccTextRectangle;
    public readonly Rectangle RankTextRectangle;
    public readonly Rectangle ModifiersTextRectangle;
    public readonly Rectangle DiffTextRectangle;
    
    public readonly float MinPlayerNameFontSize;
    public readonly float MinSongNameFontSize;

    public Size Size => FullRectangle.Size;
    public int Width => FullRectangle.Width;
    public int Height => FullRectangle.Height;

    #endregion

    #region Constructor

    public EmbedLayout(Size size) {
        FullRectangle = new Rectangle(Point.Empty, size);
        MinPlayerNameFontSize = size.Height * 0.07f;
        MinSongNameFontSize = size.Height * 0.1f;

        var center = new PointF(size.Width * 0.5f, size.Height * 0.5f);

        var avatarOrigin = center with { X = center.X - size.Width * 0.22f };
        var avatarSize = new SizeF(size.Height * 0.5f, size.Height * 0.5f);
        var avatarOverlaySize = new SizeF(avatarSize.Width * 1.5f, avatarSize.Height * 1.5f);
        AvatarRectangle = DrawingUtils.CenteredRectangle(avatarOrigin, avatarSize);
        AvatarOverlayRectangle = DrawingUtils.CenteredRectangle(avatarOrigin, avatarOverlaySize);

        var songNameOrigin = center with { Y = center.Y + size.Height * 0.35f };
        var songNameSize = new SizeF(size.Width * 0.94f, size.Height * 0.14f);
        SongNameRectangle = DrawingUtils.CenteredRectangle(songNameOrigin, songNameSize);

        var playerNameOrigin = avatarOrigin with { Y = avatarOrigin.Y - size.Height * 0.35f };
        var playerNameSize = new SizeF(size.Width * 0.5f, size.Height * 0.12f);
        PlayerNameRectangle = DrawingUtils.CenteredRectangle(playerNameOrigin, playerNameSize);

        var statsOrigin = center with { X = center.X + size.Width * 0.22f };
        
        var accTextOrigin = statsOrigin with { Y = statsOrigin.Y - size.Height * 0.18f };
        var accTextSize = new SizeF(size.Width * 0.5f, size.Height * 0.14f);
        AccTextRectangle = DrawingUtils.CenteredRectangle(accTextOrigin, accTextSize);
        
        var rankTextOrigin = statsOrigin with { Y = statsOrigin.Y - size.Height * 0.0f };
        var rankTextSize = new SizeF(size.Width * 0.5f, size.Height * 0.12f);
        RankTextRectangle = DrawingUtils.CenteredRectangle(rankTextOrigin, rankTextSize);
        
        var modifiersTextOrigin = statsOrigin with { Y = statsOrigin.Y + size.Height * 0.14f };
        var modifiersTextSize = new SizeF(size.Width * 0.5f, size.Height * 0.08f);
        ModifiersTextRectangle = DrawingUtils.CenteredRectangle(modifiersTextOrigin, modifiersTextSize);
        
        var diffTextOrigin = new PointF(size.Width * 0.88f, size.Height * 0.07f);
        var diffTextSize = new SizeF(size.Width * 0.22f, size.Height * 0.09f);
        DiffTextRectangle = DrawingUtils.CenteredRectangle(diffTextOrigin, diffTextSize);
    }

    #endregion
}