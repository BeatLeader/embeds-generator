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
    
    private static readonly Image AvatarMask = LoadImage("AvatarMask.png");
    private static readonly Image BackgroundImage = LoadImage("Background.png");
    private static readonly Image GradientMask = LoadImage("GradientMask.png");
    private static readonly Image CoverMask = LoadImage("CoverMask.png");
    private static readonly Image FinalMask = LoadImage("FinalMask.png");

    private static Image LoadImage(string fileName) {
        return new Bitmap(Path.Combine(TestFilesDirectory, fileName));
    }

    #endregion
    
    #region Main

    private static void Main() {
        var fontCollection = new PrivateFontCollection();
        fontCollection.AddFontFile(Path.Combine(TestFilesDirectory, "Teko-Medium.ttf"));
        var tekoFontFamily = fontCollection.Families[0];
        
        var embedGenerator = new EmbedGenerator(
            new Size(400, 240),
            AvatarMask,
            BackgroundImage,
            GradientMask,
            CoverMask,
            FinalMask,
            tekoFontFamily
        );
        
        var startTime = DateTime.Now;

        var image = embedGenerator.Generate(
            "Reezonate",
            "The Everlasting Calamity That Shifts The Time - Space Continuum On A Nanosecondal Basis [LEVEL 3]",
            "FC, DA, FS",
            "Ex+",
            0.9572f,
            176,
            607.58f,
            10.23f,
            CoverImage,
            AvatarImage,
            AvatarOverlayImage,
            80,
            2.0f,
            Color.Crimson,
            Color.RoyalBlue,
            Color.BlueViolet
        );

        Console.Out.WriteLine($"Generation time: {(DateTime.Now - startTime).TotalMilliseconds:F0}ms");
        
        image.Save(Path.Combine(TestFilesDirectory, "output.png"), ImageFormat.Png);
    }

    #endregion
}