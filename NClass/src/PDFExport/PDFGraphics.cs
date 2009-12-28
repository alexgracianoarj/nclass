using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using NClass.DiagramEditor;
using PdfSharp.Drawing;

namespace PDFExport
{
  /// <summary>
  /// Implements the IGraphics interface for the PDF-Sharp output.
  /// </summary>
  /// <remarks>
  /// Every point, path, rectangle and so on is measured in pixels. PDF doesn't know about
  /// pixels, but dots. One dot is the 72th of an inch. So one has to apply some scaling for
  /// alle drawn elements. This can be done with a call to ScaleTransform for example to get
  /// from pixel coordinates to dot coordinates. This solution has one disadvantage: There
  /// are a few points where PDF-Sharp scales from pixels to dots itself: fonts and images.
  /// If we don't care about this, all fonts and images would appear to small in the
  /// resulting PDF. The solution is to "unscale" those elements first. To do this, there
  /// are the two properties ScaleFont and ScaleImage.
  /// 
  /// <example>
  /// //We take the DPI of the screen from a Control
  /// Graphics gdiGraphics = (new Control()).CreateGraphics();
  /// //Apply a scaling to get from pixels to dots
  /// graphics.ScaleTransform(72.0f / gdiGraphics.DpiX, 72.0f / gdiGraphics.DpiY);
  /// //Unscale the font
  /// graphics.ScaleFont = gdiGraphics.DpiY / 72.0f;
  /// //Unscale the images
  /// graphics.ScaleImage = gdiGraphics.DpiX / 72.0f;
  /// 
  /// graphics.TakeInitialTransform();
  /// </example>
  /// 
  /// Word wrap is slightly different to the GDI word wrapping. If a single word is longer
  /// than a line, GDI breaks the word and prints the rest in a new line. Here the rest isn't
  /// printed but an ellipsis is appendet to the not fitting word.
  /// 
  /// The following points are on the to do list:
  /// - Clipping isn't very good right now. We should unerstand the regions.
  /// 
  /// There are a few things which are not perfect and won't be changed now:
  /// - Only horizontal, vertical and diagonal gradients are drawn correctly. Thats ok since
  ///   NClass dosn't use any other gradients.
  /// - Images get a gray border in pdf if they have colored (even white) pixels next to
  ///   transparent pixels. A workaround is to draw them by GDI on a surface. This surface
  ///   is white. This looks good until the backgound of the entity is also white. If  not,
  ///   images are inside a white box.
  /// - If one draws with a gradient brush and the next drawing operation uses a black brush,
  ///   this drawing is done with the gradient again. That's because the brush isn't changed in
  ///   pdf. This is a bug in PDFSharp.
  /// </remarks>
  public class PDFGraphics : IGraphics
  {
    // ========================================================================
    // Attributes

    #region === Attributes

    /// <summary>
    /// The underlying PDF-Graphics.
    /// </summary>
    private readonly XGraphics graphics;

    /// <summary>
    /// The initial state of the underlying PDF-Graphics
    /// </summary>
    private XGraphicsState initialState;

    /// <summary>
    /// Stores the current clipping region.
    /// </summary>
    private Region clippingRegion;

    /// <summary>
    /// Stores all transformations made beside the ones which are active at the initial state.
    /// </summary>
    private Matrix transform;

    #endregion

    // ========================================================================
    // Con- / Destruction

    #region === Con- / Destruction

    /// <summary>
    /// Initializes a new instnace of <c>PDFGraphics</c>.
    /// </summary>
    /// <param name="graphics">An instance of <see cref="XGraphics"/> which is used to
    ///                        create the PDF.</param>
    public PDFGraphics(XGraphics graphics)
    {
      this.graphics = graphics;
      
      transform = new Matrix();

      clippingRegion = new Region();
    }

    #endregion

    // ========================================================================
    // IGraphics implementation

    #region === IGraphics implementation

    #region --- Transform

    /// <summary>
    /// Takes the inital state.
    /// </summary>
    public void TakeInitialTransform()
    {
      initialState = graphics.Save();
      //Initial state token, so reset the transform matrix.
      transform = new Matrix();
    }

    /// <summary>
    /// Restores the state to the initial state.
    /// </summary>
    private void RestoreInitialTransform()
    {
      //Go to the initial state
      graphics.Restore(initialState);
      initialState = graphics.Save();

      //We've just restored the initial state so the there are no transformations active.
      transform = new Matrix();

      //Restore the last clipping
      ApplyClip(clippingRegion);
    }

    public Matrix Transform
    {
      get
      {
        return transform.Clone();
      }
      set
      {
        RestoreInitialTransform();
        graphics.MultiplyTransform(value);
        transform = value.Clone();
      }
    }

    public void ResetTransform()
    {
      RestoreInitialTransform();
    }

    public void TranslateTransform(float dx, float dy)
    {
      transform.Translate(dx, dy);
      graphics.TranslateTransform(dx, dy);
    }

    public void RotateTransform(float angle)
    {
      transform.Rotate(angle);
      graphics.RotateTransform(angle);
    }

    public void ScaleTransform(float sx, float sy)
    {
      transform.Scale(sx, sy);
      graphics.ScaleTransform(sx, sy);
    }

    /// <summary>
    /// Gets or sets the scale factor for all used fonts.
    /// </summary>
    public float ScaleFont
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the scale factor for all drawn images.
    /// </summary>
    public float ScaleImage
    {
      get;
      set;
    }

    #endregion

    #region --- Clip

    /// <summary>
    /// Resets the clipping.
    /// </summary>
    private void RestoreInitialClip()
    {
      //Get back to the initial state
      graphics.Restore(initialState);
      initialState = graphics.Save();

      //Restore transform
      graphics.MultiplyTransform(transform);

      clippingRegion = new Region();
    }

    public Region Clip
    {
      get { return clippingRegion; }
      set
      {
        SetClip(value, CombineMode.Replace);
      }
    }

    public RectangleF ClipBounds
    {
      get { return clippingRegion.GetBounds(graphics.Graphics); }
    }

    public void SetClip(Rectangle rect, CombineMode combineMode)
    {
      RestoreInitialClip();
      CombineClippingRegion(combineMode, new Region(rect));

      graphics.IntersectClip(rect);
    }

    public void SetClip(RectangleF rect, CombineMode combineMode)
    {
      RestoreInitialClip();
      CombineClippingRegion(combineMode, new Region(rect));

      graphics.IntersectClip(rect);
    }

    public void SetClip(GraphicsPath path, CombineMode combineMode)
    {
      RestoreInitialClip();
      CombineClippingRegion(combineMode, new Region(path));

      graphics.IntersectClip(new XGraphicsPath(path.PathData.Points, path.PathData.Types, FillModeToXFillMode(path.FillMode)));
    }

    public void SetClip(Region region, CombineMode combineMode)
    {
      RestoreInitialClip();
      CombineClippingRegion(combineMode, region);

      ApplyClip(region);
    }

    /// <summary>
    /// Applies the clipping stored in <paramref name="region"/>.
    /// </summary>
    /// <param name="region">A region which is used for clipping.</param>
    private void ApplyClip(Region region)
    {
      foreach(RectangleF rect in region.GetRegionScans(new Matrix()))
      {
        graphics.IntersectClip(rect);
      }
    }

    /// <summary>
    /// Combines the local clippingRegion with the given region.
    /// </summary>
    /// <param name="combineMode">The combine mode to use.</param>
    /// <param name="region">The region to combine with the clippingRegion.</param>
    private void CombineClippingRegion(CombineMode combineMode, Region region)
    {
      switch(combineMode)
      {
        case CombineMode.Replace:
          clippingRegion = region;
          break;
        case CombineMode.Intersect:
          clippingRegion.Intersect(region);
          break;
        case CombineMode.Union:
          clippingRegion.Union(region);
          break;
        case CombineMode.Xor:
          clippingRegion.Xor(region);
          break;
        case CombineMode.Exclude:
          clippingRegion.Exclude(region);
          break;
        case CombineMode.Complement:
          clippingRegion.Complement(region);
          break;
        default:
          throw new ArgumentOutOfRangeException("combineMode");
      }
    }

    #endregion

    #region --- Draw...

    public void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
    {
      graphics.DrawLine(PenToXPen(pen), x1, y1, x2, y2);
    }

    public void DrawLine(Pen pen, Point pt1, Point pt2)
    {
      graphics.DrawLine(PenToXPen(pen), pt1, pt2);
    }

    public void DrawLines(Pen pen, Point[] points)
    {
      graphics.DrawLines(PenToXPen(pen), points);
    }

    public void DrawRectangle(Pen pen, Rectangle rect)
    {
      graphics.DrawRectangle(PenToXPen(pen), rect);
    }

    public void DrawEllipse(Pen pen, int x, int y, int width, int height)
    {
      graphics.DrawEllipse(PenToXPen(pen), x, y, width, height);
    }

    public void DrawPolygon(Pen pen, Point[] points)
    {
      graphics.DrawPolygon(PenToXPen(pen), points);
    }

    public void DrawPath(Pen pen, GraphicsPath path)
    {
      graphics.DrawPath(PenToXPen(pen), GraphicsPathToXGraphicsPath(path));
    }

    public void DrawString(string s, Font font, Brush brush, PointF point)
    {
      graphics.DrawString(s, FontToXFont(font), BrushToXBrush(brush), point);
    }

    public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
    {
      graphics.DrawString(s, FontToXFont(font), BrushToXBrush(brush), point, StringFormatToXStringFormat(format));
    }

    public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle)
    {
      graphics.DrawString(s, FontToXFont(font), BrushToXBrush(brush), layoutRectangle);
    }

    public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
    {
      XStringFormat xFormat = StringFormatToXStringFormat(format);
      XFont xFont = FontToXFont(font);
      XBrush xBrush = BrushToXBrush(brush);
      if(format.FormatFlags == StringFormatFlags.NoWrap)
      {
        //Single line
        graphics.DrawString(TrimString(s, layoutRectangle, xFont, format.Trimming, xFormat), xFont, xBrush, layoutRectangle, xFormat);
      }
      else
      {
        //Do a word wrapping
        string[] words = s.Split(' ', '\t');
        int wordsDrawn = 0;
        double lineHeight = xFont.GetHeight(graphics);
        int lineCounter = 0;
        int wordsInLine = 0;
        StringBuilder line = new StringBuilder("");
        do
        {
          line.Append(words[wordsDrawn]);
          wordsDrawn++;
          wordsInLine++;

          bool draw = wordsDrawn == words.Length;

          if(graphics.MeasureString(line.ToString(), xFont, xFormat).Width > layoutRectangle.Width)
          {
            if((lineCounter + 2) * lineHeight > layoutRectangle.Height)
            {
              //This is definitly the last line
              for(int i = wordsDrawn; i < words.Length; i++)
              {
                line.Append(words[i]);
                line.Append(" ");
              }
              line.Length--;
              wordsDrawn = words.Length;
            }
            else if(wordsInLine > 1)
            {
              //To large: remove last word and draw the line.
              line.Length -= words[wordsDrawn - 1].Length + 1; //+ 1: Space
              wordsDrawn--;
            }
            draw = true;
          }

          if(draw)
          {
            RectangleF lineRect = new RectangleF(layoutRectangle.X, (float)(layoutRectangle.Y + lineCounter * lineHeight), layoutRectangle.Width, (float)lineHeight);
            string trimmedLine = TrimString(line.ToString(), lineRect, xFont, wordsInLine > 1 ? StringTrimming.EllipsisWord : StringTrimming.EllipsisCharacter, xFormat);
            graphics.DrawString(trimmedLine, xFont, xBrush, lineRect, xFormat);
            line = new StringBuilder();
            lineCounter++;
            wordsInLine = 0;
          }
          else
          {
            line.Append(" ");
          }
        } while(wordsDrawn < words.Length);
      }
    }

    public void DrawImage(Image image, Point point)
    {
      //Images get scaled from pixels to dots by PDF-Sharp. So we have to take care
      //that the image scaling isn't applied twice. One way would be to scale the image
      //first and then draw it. But we would get extrem blury images. So we unscale
      //everything, draw the image (don't forget to scale the coordinates), and restore
      //the old state. This is a hack.
      //What, if the ScaleImage isn't the pixel to dot scaling? That will fail.
      graphics.Save();
      graphics.ScaleTransform(ScaleImage, ScaleImage);
      PointF point2 = new PointF(point.X / ScaleImage, point.Y / ScaleImage);
      graphics.DrawImage(ImageToXImage(image), point2);
      graphics.Restore();
    }

    public void DrawImage(Image image, int x, int y)
    {
      //See DrawImage(Image image, Point point).
      graphics.Save();
      graphics.ScaleTransform(ScaleImage, ScaleImage);
      graphics.DrawImage(ImageToXImage(image), x / ScaleImage, y / ScaleImage);
      graphics.Restore();
    }

    #endregion

    #region --- Fill...

    public void FillRectangle(Brush brush, Rectangle rect)
    {
      graphics.DrawRectangle(BrushToXBrush(brush), rect);
    }

    public void FillPolygon(Brush brush, Point[] points)
    {
      //If no FillMode is given for the GDI version of FillPolygon, it uses FillMode.Alternate.
      graphics.DrawPolygon(BrushToXBrush(brush), points, XFillMode.Alternate);
    }

    public void FillEllipse(Brush brush, Rectangle rect)
    {
      graphics.DrawEllipse(BrushToXBrush(brush), rect);
    }

    public void FillEllipse(Brush brush, int x, int y, int width, int height)
    {
      graphics.DrawEllipse(BrushToXBrush(brush), x, y, width, height);
    }

    public void FillPath(Brush brush, GraphicsPath path)
    {
      graphics.DrawPath(BrushToXBrush(brush), GraphicsPathToXGraphicsPath(path));
    }

    #endregion

    #endregion

    // ========================================================================
    // Helper methods

    #region === Helper methods

    #region --- Conversion Methods

    /// <summary>
    /// Converts a GDI-GraphicsPath to a PDF-XGraphicsPath.
    /// </summary>
    /// <param name="path">The GDI-GraphicsPath to convert.</param>
    /// <returns>The converted PDF-XGraphicsPath.</returns>
    private static XGraphicsPath GraphicsPathToXGraphicsPath(GraphicsPath path)
    {
      return new XGraphicsPath(path.PathPoints, path.PathTypes, FillModeToXFillMode(path.FillMode));
    }

    /// <summary>
    /// Converts a GDI-StringFormat to a PDF-XStringFormat.
    /// </summary>
    /// <param name="format">The GDI-StringFormat to convert.</param>
    /// <returns>The converted PDF-XStringFormat.</returns>
    private static XStringFormat StringFormatToXStringFormat(StringFormat format)
    {
      return new XStringFormat
               {
                 Alignment = StringAlignmentToXStringAlignment(format.Alignment),
                 FormatFlags = StringFormatFlagsToXStringFormatFlags(format.FormatFlags),
                 LineAlignment = StringAlignmentToXLineAlignment(format.LineAlignment)
               };
    }

    /// <summary>
    /// Converts a GDI-StringAlignment to a PDF-XStringAlignment.
    /// </summary>
    /// <param name="stringAlignment">The GDI-StringAlignment to convert.</param>
    /// <returns>The converted PDF-XStringAlignment.</returns>
    private static XStringAlignment StringAlignmentToXStringAlignment(StringAlignment stringAlignment)
    {
      switch(stringAlignment)
      {
        case StringAlignment.Near:
          return XStringAlignment.Near;
        case StringAlignment.Center:
          return XStringAlignment.Center;
        case StringAlignment.Far:
          return XStringAlignment.Far;
        default:
          throw new ArgumentOutOfRangeException("stringAlignment");
      }
    }

    /// <summary>
    /// Converts a GDI-StringFormatFlags to a PDF-XStringFormatFlags.
    /// </summary>
    /// <param name="stringFormatFlags">The GDI-StringFormatFlags to convert.</param>
    /// <returns>The converted PDF-XStringFormatFlags.</returns>
    private static XStringFormatFlags StringFormatFlagsToXStringFormatFlags(StringFormatFlags stringFormatFlags)
    {
      //Nothing else is implemented...
      return XStringFormatFlags.MeasureTrailingSpaces;
    }

    /// <summary>
    /// Converts a GDI-StringAlignment to a PDF-XLineAlignment.
    /// </summary>
    /// <param name="stringAlignment">The GDI-StringAlignment to convert.</param>
    /// <returns>The converted PDF-XLineAlignment.</returns>
    private static XLineAlignment StringAlignmentToXLineAlignment(StringAlignment stringAlignment)
    {
      switch(stringAlignment)
      {
        case StringAlignment.Near:
          return XLineAlignment.Near;
        case StringAlignment.Center:
          return XLineAlignment.Center;
        case StringAlignment.Far:
          return XLineAlignment.Far;
        default:
          throw new ArgumentOutOfRangeException("stringAlignment");
      }
    }

    /// <summary>
    /// Converts a GDI-Brush to a PDF-XBrush.
    /// </summary>
    /// <remarks>
    /// Only Solid- and LinearGradientBrushes are supported. 
    /// </remarks>
    /// <param name="brush">The GDI-Brush to convert.</param>
    /// <returns>The converted PDF-XBrush.</returns>
    private static XBrush BrushToXBrush(Brush brush)
    {
      XBrush xbrush;
      SolidBrush solidBrush;
      LinearGradientBrush lgBrush;
      if((solidBrush = brush as SolidBrush) != null)
      {
        xbrush = new XSolidBrush(solidBrush.Color);
      }
      else if((lgBrush = brush as LinearGradientBrush) != null)
      {
        //There is no way to extract the angle of the gradient out of the GDI-brush.
        //It only has a transformation matrix. To create a new gradient for pdfsharp,
        //we use this matrix as follows:
        //Create a "line" (start and end point) through the rectangle at the half of
        //the heigth. The two points are p1 and p2. Transform these points with the
        //matrix. The transformed points are located on the border of the rectangle.
        PointF p1 = new PointF(lgBrush.Rectangle.Left, lgBrush.Rectangle.Top + lgBrush.Rectangle.Height / 2.0f);
        PointF p2 = new PointF(lgBrush.Rectangle.Right, lgBrush.Rectangle.Top + lgBrush.Rectangle.Height / 2.0f);
        PointF[] points = new[] { p1, p2 };
        lgBrush.Transform.TransformPoints(points);
        p1 = points[0];
        p2 = points[1];

        //The direction is ok now. But the line might be to short. That is the case if
        //the line is neither horizontal, nor vertical, nor diagonal. To fill the whole
        //rectangle with the gradient, the start and end point of the line must be located
        //outside the rectangle. To determine this gap we have to use some trigonometry.
        //This will happily never the case in NClass. So we don't have to do this here.

        xbrush = new XLinearGradientBrush(p1, p2, lgBrush.LinearColors[0], lgBrush.LinearColors[1]);
      }
      else
      {
        throw new NotImplementedException("Brush type not supported by PDFsharp.");
      }
      return xbrush;
    }

    /// <summary>
    /// Converts a GDI-Font to a PDF-XFont.
    /// </summary>
    /// <param name="font">The GDI-Font to convert.</param>
    /// <returns>The converted PDF-XFont.</returns>
    private XFont FontToXFont(Font font)
    {
      return new XFont(font.Name, font.SizeInPoints * ScaleFont, FontStyleToXFontStyle(font.Style));
    }

    /// <summary>
    /// Converts a GDI-FontStyle to a PDF-XFontStyle.
    /// </summary>
    /// <param name="fontStyle">The GDI-FontStyle to convert.</param>
    /// <returns>The converted PDF-XFontStyle.</returns>
    private static XFontStyle FontStyleToXFontStyle(FontStyle fontStyle)
    {
      if(fontStyle == (FontStyle.Bold | FontStyle.Italic))
      {
        return XFontStyle.BoldItalic;
      }
      switch(fontStyle)
      {
        case FontStyle.Regular:
          return XFontStyle.Regular;
        case FontStyle.Bold:
          return XFontStyle.Bold;
        case FontStyle.Italic:
          return XFontStyle.Italic;
        case FontStyle.Underline:
          return XFontStyle.Underline;
        case FontStyle.Strikeout:
          return XFontStyle.Strikeout;
        default:
          throw new ArgumentOutOfRangeException("fontStyle");
      }
    }

    /// <summary>
    /// Converts a GDI-Image to a PDF-XImage.
    /// </summary>
    /// <remarks>
    /// The GDI-Image gets drawn on a white background.
    /// </remarks>
    /// <param name="image">The GDI-Image to convert.</param>
    /// <returns>The converted PDF-XImage.</returns>
    private static XImage ImageToXImage(Image image)
    {
      Image image2 = new Bitmap(image.Width, image.Height);
      Graphics gfx = Graphics.FromImage(image2);
      gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, image2.Width, image2.Height);
      gfx.DrawImageUnscaled(image, 0, 0);

      return XImage.FromGdiPlusImage(image2);
    }

    /// <summary>
    /// Converts a GDI-FillMode to a PDF-XFillMode.
    /// </summary>
    /// <param name="fillMode">The GDI-FillMode to convert.</param>
    /// <returns>The converted PDF-XFillMode.</returns>
    private static XFillMode FillModeToXFillMode(FillMode fillMode)
    {
      switch(fillMode)
      {
        case FillMode.Alternate:
          return XFillMode.Alternate;
        case FillMode.Winding:
          return XFillMode.Winding;
        default:
          throw new ArgumentOutOfRangeException("fillMode");
      }
    }

    /// <summary>
    /// Converts a GDI-Pen to a PDF-XPen.
    /// </summary>
    /// <param name="pen">The GDI-Pen to convert.</param>
    /// <returns>The converted PDF-XPen.</returns>
    private static XPen PenToXPen(Pen pen)
    {
      XPen xPen = new XPen(pen.Color, pen.Width)
                    {
                      DashOffset = pen.DashOffset,
                      DashStyle = DashStyleToXDashStyle(pen.DashStyle),
                      LineCap = LineCapToXLineCap(pen.StartCap),
                      LineJoin = LineJoinToXLineJoin(pen.LineJoin),
                      MiterLimit = pen.MiterLimit
                    };

      if(pen.DashStyle == DashStyle.Custom)
      {
        xPen.DashPattern = FloatArrayToDoubleArray(pen.DashPattern);
      }

      return xPen;
    }

    /// <summary>
    /// Converts an array of floats to an array of doubles.
    /// </summary>
    /// <param name="floats">The array of floats to convert.</param>
    /// <returns>The converted array of doubles.</returns>
    private static double[] FloatArrayToDoubleArray(float[] floats)
    {
      double[] doubles = new double[floats.Length];
      for(int i = 0; i < floats.Length; i++)
      {
        doubles[i] = floats[i];
      }

      return doubles;
    }

    /// <summary>
    /// Converts a GDI-LineJoin to a PDF-XLineJoin.
    /// </summary>
    /// <remarks>
    /// PDF doesn't support LineJoin.MiterClipped so this is mapped to
    /// XLineJoin.Miter, too.
    /// </remarks>
    /// <param name="lineJoin">The GDI-LineJoin to convert.</param>
    /// <returns>The converted PDF-XLineJoin.</returns>
    private static XLineJoin LineJoinToXLineJoin(LineJoin lineJoin)
    {
      switch(lineJoin)
      {
        case LineJoin.Miter:
        case LineJoin.MiterClipped:
          return XLineJoin.Miter;
        case LineJoin.Bevel:
          return XLineJoin.Bevel;
        case LineJoin.Round:
          return XLineJoin.Round;
        default:
          return XLineJoin.Miter;
      }
    }

    /// <summary>
    /// Converts a GDI-LineCap to a PDF-XLineCap.
    /// </summary>
    /// <remarks>
    /// PDF only supports square, round and flat line caps. So all the
    /// other GDI line caps gets mapped to the best PDF line caps as follows.
    /// 
    /// <see cref="XLineCap.Square"/>
    /// <list type="bullet">
    ///   <item><description><see cref="LineCap.Square"/></description></item>
    ///   <item><description><see cref="LineCap.Triangle"/></description></item>
    ///   <item><description><see cref="LineCap.SquareAnchor"/></description></item>
    ///   <item><description><see cref="LineCap.DiamondAnchor"/></description></item>
    ///   <item><description><see cref="LineCap.ArrowAnchor"/></description></item>
    /// </list>
    /// 
    /// <see cref="XLineCap.Round"/>
    /// <list type="bullet">
    ///   <item><description><see cref="LineCap.Round"/></description></item>
    ///   <item><description><see cref="LineCap.RoundAnchor"/></description></item>
    /// </list>
    /// 
    /// <see cref="XLineCap.Flat"/>
    /// <list type="bullet">
    ///   <item><description><see cref="LineCap.Flat"/></description></item>
    ///   <item><description><see cref="LineCap.NoAnchor"/></description></item>
    ///   <item><description><see cref="LineCap.Custom"/></description></item>
    ///   <item><description><see cref="LineCap.AnchorMask"/></description></item>
    /// </list>
    /// </remarks>
    /// <param name="lineCap">The GDI-LineCap to convert.</param>
    /// <returns>The converted PDF-XLineCap.</returns>
    private static XLineCap LineCapToXLineCap(LineCap lineCap)
    {
      switch(lineCap)
      {
        case LineCap.Square:
        case LineCap.Triangle:
        case LineCap.SquareAnchor:
        case LineCap.DiamondAnchor:
        case LineCap.ArrowAnchor:
          return XLineCap.Square;

        case LineCap.Round:
        case LineCap.RoundAnchor:
          return XLineCap.Round;

        case LineCap.Flat:
        case LineCap.NoAnchor:
        case LineCap.Custom:
        case LineCap.AnchorMask:
          return XLineCap.Flat;
        default:
          return XLineCap.Flat;
      }
    }

    /// <summary>
    /// Converts a GDI-DashStyle to a PDF-XDashStyle.
    /// </summary>
    /// <param name="dashStyle">The GDI-DashStyle to convert.</param>
    /// <returns>The converted PDF-XDashStyle.</returns>
    private static XDashStyle DashStyleToXDashStyle(DashStyle dashStyle)
    {
      switch(dashStyle)
      {
        case DashStyle.Solid:
          return XDashStyle.Solid;
        case DashStyle.Dash:
          return XDashStyle.Dash;
        case DashStyle.Dot:
          return XDashStyle.Dot;
        case DashStyle.DashDot:
          return XDashStyle.DashDot;
        case DashStyle.DashDotDot:
          return XDashStyle.DashDotDot;
        case DashStyle.Custom:
          return XDashStyle.Custom;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    #endregion

    #region --- String helper

    /// <summary>
    /// Trims a string so it fits into the rectangle. Only one line is trimed.
    /// </summary>
    /// <remarks>
    /// The StringTrimming.EllipsisPath is handeled as StringTrimming.EllipsisCharacter.
    /// </remarks>
    /// <param name="s">The string to trim.</param>
    /// <param name="rectangle">A rectangle which determines the maximum width of the string.</param>
    /// <param name="font">The font which is used to print the text.</param>
    /// <param name="stringTrimming">A StringTrimming which should be used. See remarks.</param>
    /// <param name="xStringFormat">The XStringFormat which will be used when drawing the string.</param>
    /// <returns>The (possible) trimmed string.</returns>
    private string TrimString(string s, RectangleF rectangle, XFont font, StringTrimming stringTrimming, XStringFormat xStringFormat)
    {
      if(graphics.MeasureString(s, font, xStringFormat).Width <= rectangle.Width
         || stringTrimming == StringTrimming.None)
      {
        return s;
      }

      bool ellipsis = stringTrimming == StringTrimming.EllipsisCharacter
                   || stringTrimming == StringTrimming.EllipsisWord
                   || stringTrimming == StringTrimming.EllipsisPath;
      bool word = stringTrimming == StringTrimming.Word
               || stringTrimming == StringTrimming.EllipsisWord;

      string result = ellipsis ? s + "..." : s;
      do
      {
        if(word)
        {
          int pos = result.LastIndexOf(' ');
          result = (pos <= 0 ? "" : result.Substring(0, pos));
        }
        else
        {
          result = result.Substring(0, (ellipsis ? result.Length - 4 : result.Length - 1));
        }
        result = ellipsis ? result + "..." : result;
      } while(graphics.MeasureString(result, font, xStringFormat).Width > rectangle.Width
              || s.Length <= 0);

      return result;
    }

    #endregion

    #endregion
  }
}
