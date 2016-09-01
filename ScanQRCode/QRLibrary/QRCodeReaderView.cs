using System;
using UIKit;
using AVFoundation;
using Foundation;
using CoreGraphics;
using CoreFoundation;

namespace TestScanQRCode
{
	public class QRCodeReaderView : UIView
	{
		QRCodeReaderViewController thisController;

		CGRect scanViewRect;
		CGRect cropRect;
		ScanAnimateView scanAnimateView;

		NSError error;
		AVCaptureDevice device;
		AVCaptureSession session;
		AVCaptureDeviceInput videoInput;
		AVCaptureMetadataOutput metadataOutput;
		AVCaptureVideoPreviewLayer previewLayer;

		public void StartScan()
		{
			if (session != null) {
				session.StartRunning ();
				scanAnimateView.StartAnimation ();
			}
			else {
				UIAlertView alert = new UIAlertView ("Init Error", "Don't have access to camera, please allow and restart this view", null, "OK", null);
				alert.Clicked += delegate {
					thisController.DismissViewController(true,null);
				};
				alert.Show ();
			}
		}

		public void StopScan()
		{
			if(null != session)
				session.StopRunning ();
		}

		public QRCodeReaderView (QRCodeReaderViewController _thisController)
		{
			this.thisController = _thisController;

			this.BackgroundColor = UIColor.White;
	
			scanAnimateView = new ScanAnimateView ();
			this.AddSubview (scanAnimateView);

			UIButton btnCancel = new UIButton (UIButtonType.Custom);
			btnCancel.SetTitle ("Cancel", UIControlState.Normal);
			btnCancel.SetTitleColor (UIColor.LightGray, UIControlState.Highlighted);
			btnCancel.Frame = new CGRect (20, 40, 80, 20);
			this.Add (btnCancel);
			btnCancel.TouchUpInside += delegate {
				thisController.Close();
			};

			UIButton btnFlash = new UIButton (UIButtonType.Custom);
			btnFlash.SetTitle ("Flash", UIControlState.Normal);
			btnFlash.SetTitleColor (UIColor.LightGray, UIControlState.Highlighted);
			btnFlash.Frame = new CGRect (UIScreen.MainScreen.Bounds.Width-100, 40, 80, 20);
			this.Add (btnFlash);
			btnFlash.TouchUpInside += delegate {
				device.LockForConfiguration(out error);
				if(device.FlashMode == AVCaptureFlashMode.On){
					device.TorchMode = AVCaptureTorchMode.Off;
					device.FlashMode = AVCaptureFlashMode.Off;
				}
				else{
					device.TorchMode = AVCaptureTorchMode.On;
					device.FlashMode = AVCaptureFlashMode.On;
				}
				device.UnlockForConfiguration();
			};

			Setup ();
		}

		public void HideLayer()
		{
			if (null != previewLayer) {
				previewLayer.Hidden = true;
				this.PerformSelector (new ObjCRuntime.Selector ("showLayer"), null, 1);
			}
		}

		[Export("showLayer")]
		void ShowLayer()
		{
			previewLayer.Hidden = false;
		}
		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				base.Frame = value;

				CGSize size = this.Bounds.Size;

				if (null != scanAnimateView) {
					nfloat svLength = UIScreen.MainScreen.Bounds.Width * 0.618f;
					nfloat svX = (size.Width - svLength) / 2;
					nfloat svY = (size.Height - svLength) / 2;
					scanViewRect = new CGRect (svX, svY, svLength, svLength);
					scanAnimateView.Frame = scanViewRect;
				}

				nfloat p1 = size.Height / size.Width;
				nfloat p2 = 1920 / 1080f;
				if (p1 < p2) {
					nfloat fixHeight = size.Width * 1920 / 1080f;
					nfloat fixPadding = (fixHeight - size.Height) / 2;

					nfloat theX = (scanViewRect.Y + fixPadding) / fixHeight;
					nfloat theY = scanViewRect.X / size.Width;
					nfloat theWidth = scanViewRect.Size.Height / fixHeight;
					nfloat theHeight = scanViewRect.Size.Width / size.Width;

					cropRect = new CGRect (theX, theY, theWidth, theHeight);
				}
				else {
					nfloat fixWidth = size.Width * 1080 / 1920f;
					nfloat fixPadding = (fixWidth - size.Width) / 2;

					nfloat theX = scanViewRect.Y / size.Height;
					nfloat theY = (scanViewRect.X + fixPadding) / fixWidth;
					nfloat theWidth = scanViewRect.Size.Height / size.Height;
					nfloat theHeight = scanViewRect.Size.Width / fixWidth;

					cropRect = new CGRect (theX, theY, theWidth, theHeight);
				}

				if (null != metadataOutput) {
					metadataOutput.RectOfInterest = cropRect;
				}
			}
		}

		private void Setup()
		{
			AVAuthorizationStatus authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			switch (authorizationStatus) {
			case AVAuthorizationStatus.NotDetermined:{
					AVCaptureDevice.RequestAccessForMediaType (AVMediaType.Video, new AVRequestAccessStatus (delegate(bool accessGranted) {
						if(accessGranted){
							SetupCapture ();
						}
						else{
							Console.WriteLine ("Access denied");
						}
					}));
					break;
				}
			case AVAuthorizationStatus.Authorized:{
					SetupCapture ();
					break;
				}
			case AVAuthorizationStatus.Restricted:{
					Console.WriteLine ("Access denied");
					break;
				}
			default:{
					break;
				}
			}
		}

		public void SetupCapture()
		{
			this.session = new AVCaptureSession();
			device = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			//更改这个设置的时候必须先锁定设备，修改完后再解锁，否则崩溃
//			device.LockForConfiguration(out error);
//			//设置闪光灯为自动
//			device.FlashMode = AVCaptureFlashMode.Auto;
//			device.UnlockForConfiguration();

			this.videoInput = new AVCaptureDeviceInput(device,out error);
			if (null != error) {
				Console.WriteLine ("error="+error);
			}

			this.metadataOutput = new AVCaptureMetadataOutput();

			if(this.session.CanAddInput(this.videoInput)){
				this.session.AddInput(this.videoInput);
			}
			if(this.session.CanAddOutput(this.metadataOutput)){
				this.session.AddOutput (this.metadataOutput);
			}

			// 创建dispatch queue.
			DispatchQueue dispatchQueue = new DispatchQueue ("kScanQRCodeQueueName");
			metadataOutput.SetDelegate (QRCodeGlobalObject.TheAppDel, dispatchQueue);

			// 设置元数据类型 AVMetadataObjectTypeQRCode
			metadataOutput.MetadataObjectTypes = AVMetadataObjectType.QRCode;

			//初始化预览图层
			this.previewLayer = new AVCaptureVideoPreviewLayer(this.session);
			this.previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
			this.previewLayer.Frame = new CGRect (0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
			this.Layer.MasksToBounds = true;
			this.Layer.InsertSublayer (previewLayer,0);
		}
	}
}

