using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Tonality.Resources;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using Tonality.ViewModels;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using GalaSoft.MvvmLight.Ioc;
using Tonality.Services.Interfaces;

namespace Tonality
{

    public partial class MainPage : PhoneApplicationPage
    {
        public string FilePath { get; set; }

        private Stream audioStream;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // SoundModel svm = new SoundModel();
            // svm.LoadData();
            //this.LongList1.ItemsSource = Locator.Items;
            //this.LongList2.ItemsSource = svm.Messengers.Items;
            //this.LongList3.ItemsSource = svm.GamesMsc.Items;
            //this.LongList4.ItemsSource = svm.Android.Items;
            //this.LongList5.ItemsSource = svm.Entertainment.Items;
            //this.LongList6.ItemsSource = svm.NewAdds.Items;
        }


        // Check to make sure there are enough space available on the phone
        // in order to save the image that we are downloading on to the phone
        private bool IsSpaceIsAvailable(long spaceReq)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {

                long spaceAvail = store.AvailableFreeSpace;
                if (spaceReq > spaceAvail)
                {
                    return false;
                }
                return true;
            }
        }


        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector selector = sender as LongListSelector;

            // verifying our sender is actually a LongListSelector
            if (selector == null)
                return;

            SoundData data = selector.SelectedItem as SoundData;

            // verifying our sender is actually SoundData
            if (data == null)
                return;


            if (data.IsDownloaded)
            {
                this.PlaySound(IsolatedStorageFile.GetUserStoreForApplication().OpenFile(data.SavePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            else
            {
                if (!SimpleIoc.Default.GetInstance<INetworkService>().IsConnectionAvailable)
                {
                    MessageBox.Show("You need an internet connection to download this sound.");
                }
                else
                {
                    WebClient client = new WebClient();

                    client.DownloadProgressChanged += (senderClient, args) =>
                        {
                            SoundData passedData = (SoundData)args.UserState;
                            Dispatcher.BeginInvoke(() =>
                                {
                                    passedData.DownloadProgress = args.ProgressPercentage;
                                });
                        };

                    client.OpenReadCompleted += (senderClient, args) =>
                    {
                        if (args.Error != null)
                        {
                            MessageBox.Show("Error downloading sound", args.Error.Message, MessageBoxButton.OK);
                        }
                        else
                        {
                            SoundData passedData = (SoundData)args.UserState;
                            using (IsolatedStorageFileStream fileStream = IsolatedStorageFile.GetUserStoreForApplication().CreateFile(passedData.SavePath))
                            {
                                args.Result.Seek(0, SeekOrigin.Begin);
                                args.Result.CopyTo(fileStream);

                                this.PlaySound(fileStream);
                                passedData.Status = DownloadStatus.Downloaded;
                            }

                            args.Result.Close();
                        }
                    };

                    client.OpenReadAsync(new Uri(data.FilePath), data);
                    data.Status = DownloadStatus.Downloading;
                }
            }

            selector.SelectedItem = null;
        }

        private void Review_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask objReviewTask = new MarketplaceReviewTask();
            objReviewTask.Show();
        }

        private void Email_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask EmailComposeTask = new EmailComposeTask();
            EmailComposeTask.To = "BBSounds@outlook.com";
            EmailComposeTask.Show();

        }

        private void Share_Click(object sender, EventArgs e)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Check out this app";
            task.Body = "Check out Tonality for free popular notification sounds http://www.windowsphone.com/s?appid=7ac59477-9b21-41a7-9433-9f9f602e8f77";
            task.Show();
        }

        private void fb_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=7ac59477-9b21-41a7-9433-9f9f602e8f77", UriKind.Absolute);
            shareLinkTask.Message = "Check out Tonality";
            shareLinkTask.Show();
        }

        private void twitter_Click(object sender, EventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://www.windowsphone.com/s?appid=7ac59477-9b21-41a7-9433-9f9f602e8f77", UriKind.Absolute);
            shareLinkTask.Message = "Check out Tonality!";
            shareLinkTask.Show();
        }

        private void TonalityRedux_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketTask = new MarketplaceDetailTask();
            marketTask.ContentIdentifier = "bc0a81ec-f48e-4f02-84f0-4a041afa86da";
            marketTask.ContentType = MarketplaceContentType.Applications;

            marketTask.Show();
        }

        private void Paid_Click(object sender, EventArgs e)
        {
            MarketplaceDetailTask marketTask = new MarketplaceDetailTask();
            marketTask.ContentIdentifier = "0940431a-2eec-4d19-bed6-41379538da76";
            marketTask.ContentType = MarketplaceContentType.Applications;

            marketTask.Show();
        }

        private void adcontrol_adrefreshed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Adcontrol new ad received");
        }

        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Ad Error : ({0}) {1}", e.ErrorCode, e.Error);
        }

        private void PlaySound(FileStream soundStream)
        {
            if (this.audioStream != null)
            {
                this.audioStream.Close();
                this.audioStream.Dispose();
            }

            audioStream = soundStream;
            AudioPlayer.SetSource(soundStream);
            AudioPlayer.Play();
        }
    }
}







