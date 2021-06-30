﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProjectBangaloreTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();

        }
        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileOpenPicker.FileTypeFilter.Add(".jpg");
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
            StorageFile selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();
            if (selectedStorageFile != null)
            {
                await DisplayImage(selectedStorageFile);


                //Use Squeezenet model to classify image
                var imageClasses = await ProjectBangalore.SqueezeNetObjectDetectionModel.ClassifyImage(selectedStorageFile, 3);


                //Use YOLOv4 to detect objects
                var listOfObjects = await ProjectYOLO.YOLOObjectDetectionModel.DetectObjects(selectedStorageFile);

                StatusBlock.Text = "";
                for (int i = 0; i < imageClasses.Count; ++i)
                {
                    if (i == 0)
                    {
                        StatusBlock.Text = "SqueezeNet Results: \n"; ;
                    }

                    StatusBlock.Text += imageClasses[i].category + "with confidence "+ imageClasses[i].confidence + "\n";

                }

                for (int i = 0; i < listOfObjects.Count; ++i)
                {
                    if(i == 0)
                    {
                       StatusBlock.Text += "Yolo Results : \n";
                    }

                    StatusBlock.Text += listOfObjects[i].label + "\n";

                }
            }

        }

        private async Task DisplayImage(StorageFile selectedStorageFile)
        {
            SoftwareBitmap softwareBitmap;
            using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
            {
                // Create the decoder from the stream 
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // Get the SoftwareBitmap representation of the file in BGRA8 format
                softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
            }
            // Display the image
            SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
            await imageSource.SetBitmapAsync(softwareBitmap);
            UIPreviewImage.Source = imageSource;
        }
    }
}
