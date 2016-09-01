using System;
using UIKit;
using AVFoundation;
using CoreGraphics;
using Foundation;
using CoreFoundation;

namespace TestScanQRCode
{
	public delegate void QRCodeReaderCallback(string result);
	public class QRCodeReaderViewController : UIViewController
	{
		private QRCodeReaderView readerView;
		private QRCodeReaderCallback callback;
		private static QRCodeReaderViewController intance;

		public static QRCodeReaderViewController Intance()
		{
			if (null == intance)
				intance = new QRCodeReaderViewController ();
			return intance;
		}

		private QRCodeReaderViewController ()
		{
			QRCodeGlobalObject.TheQRCodeReaderViewController = this;

			readerView = new QRCodeReaderView (this);
			this.View = readerView;
		}

		public override void ViewDidAppear (bool animated)
		{
			readerView.StartScan ();
		}

		public override void ViewDidDisappear (bool animated)
		{
			readerView.StopScan ();
		}

		public void ReturnResult(string result)
		{
			callback.Invoke (result);
			Close ();
		}

		public void Show(UINavigationController navController,QRCodeReaderCallback _callback)
		{
			readerView.HideLayer ();
			callback = _callback;
			navController.PresentViewController(this,true,null);
		}

		public void Close()
		{
			this.DismissViewController (true, delegate {
				QRCodeGlobalObject.TheAppDel.CanReceiveData = true;
			});
		}
	}
}

