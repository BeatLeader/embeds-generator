using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace EmbedGenerator;

internal static class Program {
    #region ExampleData

    private const string TestFilesDirectory = @"D:\Projects\Beat Saber\BeatLeader\EmbedGenerator\TestFiles";

    private static readonly Image AvatarImage = LoadImage("Avatar.png");
    private static readonly Image AvatarOverlayImage = LoadImage("AvatarOverlay.png");
    private static readonly Image CoverImage = LoadImage("Cover.png");

    private static readonly Image StarImage = LoadImage("Star.png");
    private static readonly Image AvatarMask = LoadImage("AvatarMask.png");
    private static readonly Image AvatarShadow = LoadImage("AvatarShadow.png");
    private static readonly Image BackgroundImage = LoadImage("Background.png");
    private static readonly Image GradientMask = LoadImage("GradientMask.png");
    private static readonly Image GradientMaskBlurred = LoadImage("GradientMaskBlurred.png");
    private static readonly Image CoverMask = LoadImage("CoverMask.png");
    private static readonly Image FinalMask = LoadImage("FinalMask.png");

    private static Image LoadImage(string fileName) {
        return new Bitmap(Path.Combine(TestFilesDirectory, fileName));
    }

    #endregion

    #region Main

    private static void Main() {
        var fontCollection = new PrivateFontCollection();
        fontCollection.AddFontFile(Path.Combine(TestFilesDirectory, "Teko-SemiBold.ttf"));
        var tekoFontFamily = fontCollection.Families[0];

        var scale = 1.0f;
        var embedGenerator = new EmbedGenerator(
            new Size((int)(500 * scale), (int)(300 * scale)),
            StarImage,
            AvatarMask,
            AvatarShadow,
            BackgroundImage,
            GradientMask,
            GradientMaskBlurred,
            CoverMask,
            FinalMask,
            tekoFontFamily
        );

        Image? image = default;

        for (var i = 0; i < 5; i++) {
            var startTime = DateTime.Now;

            image = embedGenerator.Generate(
                "Reezonate",
                "Reeverie on the Onyx",
                "FC, DA, FS",
                "Expert+",
                0.9573f,
                13,
                607.58f,
                123.352f,
                CoverImage,
                AvatarImage,
                AvatarOverlayImage,
                // null,
                80,
                2.0f,
                Color.FromArgb(255, 255, 155, 20),
                Color.FromArgb(255, 0, 145, 255),
                Color.BlueViolet
            );

            Console.Out.WriteLine($"Generation time: {(DateTime.Now - startTime).TotalMilliseconds:F0}ms");
        }

        image?.Save(Path.Combine(TestFilesDirectory, "output.png"), ImageFormat.Png);
    }

    #endregion
}