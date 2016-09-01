using System;
using UIKit;
using CoreGraphics;
using Foundation;

namespace TestScanQRCode
{
	public class MainViewController : UIViewController
	{
		public MainViewController ()
		{
			this.View.BackgroundColor = UIColor.Gray;
		}

		public override void ViewDidLoad ()
		{
			this.NavigationItem.Title = "Home";
			nfloat locationX = (UIScreen.MainScreen.Bounds.Width - 80) / 2;
			nfloat locationY = UIScreen.MainScreen.Bounds.Height - 50;

			UIButton btnScan = new UIButton (UIButtonType.Custom);
			btnScan.Frame = new CGRect (locationX, locationY,80, 20);
			btnScan.SetTitle ("Sacn", UIControlState.Normal);
			btnScan.SetTitleColor (UIColor.Black, UIControlState.Normal);
			btnScan.TouchUpInside += delegate {
				this.ShowQRCodeReaderViewController((result)=>{
					UIAlertView alert = new UIAlertView("Result",result,null,"OK",null);
					alert.Show();
				});
			};
			this.Add (btnScan);

			nfloat padding = 20;
			UITextField tf = new UITextField ();
			tf.Frame = new CGRect (20, 100, UIScreen.MainScreen.Bounds.Width - 2 * padding, 40);
			tf.Text = "Tap ImageView to create QR code";
			tf.BackgroundColor = UIColor.White;
			this.Add (tf);

			nfloat length = UIScreen.MainScreen.Bounds.Width * 0.5f;
			nfloat imageVLocationX = (UIScreen.MainScreen.Bounds.Width - length)/2;
			UIImageView showQRCodeImgView = new UIImageView (new CGRect (imageVLocationX, tf.Frame.Y + tf.Frame.Height + 50, length, length));
			showQRCodeImgView.BackgroundColor = UIColor.White;
			this.Add (showQRCodeImgView);

			UITapGestureRecognizer tapGR = new UITapGestureRecognizer (() => {
				Console.WriteLine("Create a new code according to the text");
				tf.ResignFirstResponder();
				string sourceStr = tf.Text.Trim();
				showQRCodeImgView.Image = QRCodeCreater.Instance().GetQRCodeImageByString(sourceStr,showQRCodeImgView.Frame.Width);
			});
			this.View.AddGestureRecognizer (tapGR);
		}
	}
}

