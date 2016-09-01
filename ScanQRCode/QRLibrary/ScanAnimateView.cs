using System;
using UIKit;
using CoreGraphics;

namespace TestScanQRCode
{
	public class ScanAnimateView : UIView
	{
		nfloat padding = 10;
		nfloat lineWidth = 2;
		UIView scanLine = new UIView();

		public override CoreGraphics.CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				base.Frame = value;
			}
		}

		public ScanAnimateView ()
		{
			this.BackgroundColor = UIColor.Clear;
			scanLine.BackgroundColor = UIColor.Green;
		}

		public void StartAnimation()
		{
			scanLine.Frame = new CGRect (padding, padding, this.Frame.Width - 2 * padding, lineWidth);
			this.Add (scanLine);
			UIView.Animate (2, 0, UIViewAnimationOptions.Repeat | UIViewAnimationOptions.CurveLinear | UIViewAnimationOptions.Autoreverse, delegate {
				CGRect tmp = scanLine.Frame;
				tmp.Y = this.Frame.Height - lineWidth - padding;
				scanLine.Frame = tmp;
			}, null);
		}

		public override void Draw (CGRect rect)
		{
			CGContext con = UIGraphics.GetCurrentContext ();

			nfloat length = rect.Width * 0.1f;
			con.SetLineWidth (3);
			con.SetStrokeColor (UIColor.Green.CGColor);

			con.MoveTo (0, length);
			con.AddLineToPoint (0, 0);
			con.AddLineToPoint (length, 0);
			con.StrokePath ();

			con.MoveTo (rect.Width-length, 0);
			con.AddLineToPoint (rect.Width, 0);
			con.AddLineToPoint (rect.Width, length);
			con.StrokePath ();

			con.MoveTo (rect.Width, rect.Height-length);
			con.AddLineToPoint (rect.Width, rect.Height);
			con.AddLineToPoint (rect.Width-length, rect.Height);
			con.StrokePath ();

			con.MoveTo (length, rect.Height);
			con.AddLineToPoint (0, rect.Height);
			con.AddLineToPoint (0, rect.Height-length);
			con.StrokePath ();
		}
	}
}

