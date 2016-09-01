# QRCodeSample
An easy application for create and scan QR code on iOS using Xamarin.iOS

You can create a QRCode by using this code:

    UIimage qrCodeImage = QRCodeCreater.Instance().GetQRCodeImageByString(sourceStr,showQRCodeImgView.Frame.Width);

And you can scan a QRCode by using the Extension Methods in every UIViewController like:

    this.ShowQRCodeReaderViewController((result)=>{
		  UIAlertView alert = new UIAlertView("Result",result,null,"OK",null);
		  alert.Show();
		});

Or the origin method, like:

    QRCodeReaderViewController.Intance().Show(controller.NavigationController,(result)=>{
		  UIAlertView alert = new UIAlertView("Result",result,null,"OK",null);
		  alert.Show();
		});
		
This are some screenshot showing the effect:

Generate a image:
![](https://github.com/lwnwowone/QRCodeSample/raw/master/sample1.PNG)

Scanning:
![](https://github.com/lwnwowone/QRCodeSample/raw/master/sample2.PNG)

Result pop up:
![](https://github.com/lwnwowone/QRCodeSample/raw/master/sample3.PNG)
