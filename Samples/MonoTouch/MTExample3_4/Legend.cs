using System;
using System.Drawing;
using CoreGraphics;

namespace MTExample3_4 {
	public class Legend {
		bool isLegendVisible;
		Color textColor;
		LegendPositionEnum legendPosition;
		bool isBorderVisible;
		Color legendBackColor;
		Color legendBorderColor;
		Font legendFont;

		public Legend ()
		{
			legendPosition = LegendPositionEnum.NorthEast;
			textColor = Color.Black;
			isLegendVisible = false;
			isBorderVisible = true;
			legendBackColor = Color.White;
			legendBorderColor = Color.Black;
			legendFont = new Font ("Arial", 8, FontStyle.Regular);
		}

		public Font LegendFont {
			get {
				return legendFont;
			}
			set {
				legendFont = value;
			}
		}

		public Color LegendBackColor {
			get {
				return legendBackColor;
			}
			set {
				legendBackColor = value;
			}
		}

		public Color LegendBorderColor {
			get {
				return legendBorderColor;
			}
			set {
				legendBorderColor = value;
			}
		}

		public bool IsBorderVisible {
			get {
				return isBorderVisible;
			}
			set {
				isBorderVisible = value;
			}
		}

		public LegendPositionEnum LegendPosition {
			get {
				return legendPosition;
			}
			set {
				legendPosition = value;
			}
		}

		public Color TextColor {
			get {
				return textColor;
			}
			set {
				textColor = value;
			}
		}

		public bool IsLegendVisible {
			get {
				return isLegendVisible;
			}
			set {
				isLegendVisible = value;
			}
		}

		public enum LegendPositionEnum {
			North,
			NorthWest,
			West,
			SouthWest,
			South,
			SouthEast,
			East,
			NorthEast
		}

		public void AddLegend(Graphics g, DataCollection dc, ChartStyle cs)
		{
			if (dc.DataSeriesList.Count < 1)
				return;

			if (!IsLegendVisible)
				return;

			int numberOfDataSeries = dc.DataSeriesList.Count;
			string[] legendLabels = new string[dc.DataSeriesList.Count];
			int n = 0;
			foreach (DataSeries ds in dc.DataSeriesList) {
				legendLabels[n] = ds.SeriesName;
				n++;
			}

			float offSet = 10;
			float xc = 0f;
			float yc = 0f;
			CGSize size = g.MeasureString (legendLabels[0], LegendFont);
			nfloat legendWidth = size.Width;
			for (int i = 0; i < legendLabels.Length; i++) {
				size = g.MeasureString (legendLabels[i], LegendFont);
				nfloat tempWidth = size.Width;
				if (legendWidth < tempWidth)
					legendWidth = tempWidth;
			}
			legendWidth = legendWidth + 50.0f;
			var hWidth = (float)(legendWidth / 2);
			float legendHeight = 18.0f * numberOfDataSeries;
			float hHeight = legendHeight / 2;

			switch (LegendPosition) {
				case LegendPositionEnum.East:
					xc = cs.PlotArea.X + cs.PlotArea.Width - offSet - hWidth;
					yc = cs.PlotArea.Y + cs.PlotArea.Height / 2;
					break;
				case LegendPositionEnum.North:
					xc = cs.PlotArea.X + cs.PlotArea.Width / 2;
					yc = cs.PlotArea.Y + offSet + hHeight;
					break;
				case LegendPositionEnum.NorthEast:
					xc = cs.PlotArea.X + cs.PlotArea.Width - offSet - hWidth;
					yc = cs.PlotArea.Y + offSet + hHeight;
					break;
				case LegendPositionEnum.NorthWest:
					xc = cs.PlotArea.X + offSet + hWidth;
					yc = cs.PlotArea.Y + offSet + hHeight;
					break;
				case LegendPositionEnum.South:
					xc = cs.PlotArea.X + cs.PlotArea.Width / 2;
					yc = cs.PlotArea.Y + cs.PlotArea.Height - offSet - hHeight;
					break;
				case LegendPositionEnum.SouthEast:
					xc = cs.PlotArea.X + cs.PlotArea.Width - offSet - hWidth;
					yc = cs.PlotArea.Y + cs.PlotArea.Height - offSet - hHeight;
					break;
				case LegendPositionEnum.SouthWest:
					xc = cs.PlotArea.X + offSet + hWidth;
					yc = cs.PlotArea.Y + cs.PlotArea.Height - offSet - hHeight;
					break;
				case LegendPositionEnum.West:
					xc = cs.PlotArea.X + offSet + hWidth;
					yc = cs.PlotArea.Y + cs.PlotArea.Height / 2;
					break;
			}
			DrawLegend (g, xc, yc, hWidth, hHeight, dc, cs);
		}

		private void DrawLegend(Graphics g, float xCenter, float yCenter,
			float hWidth, float hHeight, DataCollection dc, ChartStyle cs)
		{
			float spacing = 8.0f;
			float textHeight = 8.0f;
			float htextHeight = textHeight / 2.0f;
			float lineLength = 30.0f;
			Rectangle legendRectangle;
			var aPen = new Pen (LegendBorderColor, 1f);
			var aBrush = new SolidBrush (LegendBackColor);

			if (isLegendVisible) {
				legendRectangle = new Rectangle (
					(int)xCenter - (int)hWidth, (int)yCenter - (int)hHeight,
					(int)(2.0f * hWidth), (int)(2.0f * hHeight)
				);
				g.FillRectangle (aBrush, legendRectangle);
				if (IsBorderVisible)
					g.DrawRectangle (aPen, legendRectangle);

				int n = 1;
				foreach (DataSeries ds in dc.DataSeriesList) {
					// Draw lines and symbols:
					float xText = legendRectangle.X + 2 * spacing + lineLength;
					float yText = legendRectangle.Y + n * spacing + (2 * n - 1) * htextHeight;
					aPen = new Pen(ds.LineStyle.LineColor, ds.LineStyle.Thickness);
					aPen.DashStyle = ds.LineStyle.Pattern;
					var ptStart = new PointF (legendRectangle.X + spacing, yText);
					var ptEnd = new PointF (legendRectangle.X + spacing + lineLength, yText);

					g.DrawLine (aPen, ptStart, ptEnd);
					// Draw text:
					var sFormat = new StringFormat ();
					sFormat.Alignment = StringAlignment.Near;
					g.DrawString (ds.SeriesName, LegendFont, new SolidBrush(textColor), new PointF (xText, yText - 8), sFormat);
					n++;
				}
			}
			aPen.Dispose ();
			aBrush.Dispose ();
		}
	}
}
