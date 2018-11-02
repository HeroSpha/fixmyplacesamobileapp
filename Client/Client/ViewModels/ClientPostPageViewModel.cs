using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using SharedCode.Models;
using System.Threading.Tasks;
using Plugin.Media;
using System.Net.Http.Headers;
using System.IO;
using SharedCode.Helpers;
using Flurl.Http;
using Prism.Navigation;
using Prism.Events;
using SharedCode.Events;
using Flurl;

namespace Client.ViewModels
{
    public class ClientPostPageViewModel : BindableBase, INavigatingAware
    {
        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { SetProperty(ref enabled, value); }
        }
        private string locationTitle = "ion-location";
        public string LocationTitle
        {
            get { return locationTitle; }
            set { SetProperty(ref locationTitle, value); }
        }
        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
     
        private string title1 = "ion-camera";
        public string Title1
        {
            get { return title1; }
            set { SetProperty(ref title1, value); }
        }
        private string title2 = "ion-camera";
        public string Title2
        {
            get { return title2; }
            set { SetProperty(ref title2, value); }
        }
        private string title3 = "ion-camera";
        public string Title3
        {
            get { return title3; }
            set { SetProperty(ref title3, value); }
        }

        public StreamContent File1 { get; set; }
        public StreamContent File2 { get; set; }
        public StreamContent File3 { get; set; }


        public string _path1 { get; set; } = "";
        public string _path2 { get; set; } = "";
        public string _path3 { get; set; } = "";


        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private ImageSource _image1;
        public ImageSource Image1
        {
            get { return _image1; }
            set
            {
                SetProperty(ref _image1, value);
                if (Image1 != null)
                {
                    Title1 = "ion-trash-b";
                }
            }
        }
        private ImageSource _image2;
        public ImageSource Image2
        {
            get { return _image2; }
            set
            {
                SetProperty(ref _image2, value);
                if (Image2 != null)
                {
                    Title2 = "ion-trash-b";
                }
            }
        }
        private ImageSource _image3;
        public ImageSource Image3
        {
            get { return _image3; }
            set
            {
                SetProperty(ref _image3, value);
                if (Image3 != null)
                {
                    Title3 = "ion-trash-b";
                }
            }
        }
        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private Category _category;
        public Category Category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private List<Category> _categoryList;
        public List<Category> CategoryList
        {
            get { return _categoryList; }
            set { SetProperty(ref _categoryList, value); }
        }

        MultipartFormDataContent ImageFiles1;
        MultipartFormDataContent ImageFiles2;
        MultipartFormDataContent ImageFiles3;
        public DelegateCommand SaveCommand { get; set; }
       
        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set { SetProperty(ref _canSave, value); }
        }
        public DelegateCommand ThirdImageCommand { get; set; }
        public DelegateCommand SecondImageCommand { get; set; }
        public DelegateCommand FirstImageCommand { get; set; }
       // public Position? Position { get; set; }
        public DelegateCommand LocationCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public ClientPostPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            
            FirstImageCommand = new DelegateCommand(FirstImage);
            SecondImageCommand = new DelegateCommand(SecondImage);
            ThirdImageCommand = new DelegateCommand(ThirdImage);
            LocationCommand = new DelegateCommand(LocationSwitch);

            SaveCommand = new DelegateCommand(Save).ObservesCanExecute(() => CanSave);
            GetCategories().ConfigureAwait(true);
           // _eventAggregator.GetEvent<UpdateLocationEvent>().Subscribe(MyLocation);
            _eventAggregator.GetEvent<CancelLocationEvent>().Subscribe(CancelLocation);
        }

        private async void LocationSwitch()
        {
            switch(LocationTitle)
            {
                case "ion-location": { GetLocation(); } break;
                case "ion-trash-b":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove location?", "Location", "Remove", "Cancel", null);
                        if(result)
                        {
                            CancelLocation(true);
                           
                        }
                    } break;
            }
        }
        private void CancelLocation(bool obj)
        {
           if(obj)
            {
               //if(Position != null)
               // {
               //     Position = null;
               //     Acr.UserDialogs.UserDialogs.Instance.Toast("Location removed");
               //     LocationTitle = "ion-location";
               // }
               //else
               // {
               //     Acr.UserDialogs.UserDialogs.Instance.Toast("Location is null");
               // }

            }
        }

        //private void MyLocation(Position obj)
        //{
        //    Position = obj;
        //    LocationTitle = "ion-trash-b";
        //    Acr.UserDialogs.UserDialogs.Instance.Toast("Location added");
        //}

        private async void GetLocation()
        {
            try
            {
                await _navigationService.NavigateAsync("ClientLocationPage");
            }
            catch (Exception ex)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        private async void ThirdImage()
        {
            switch (Title3)
            {
                case "ion-camera":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("Choose", "Cancel", "OK", null, "Take a photo", "Browse photos");
                        switch (result)
                        {
                            case "Take a photo":
                                {
                                   await TakePhoto("3");
                                }
                                break;
                            case "Browse photos":
                                {
                                    await BrowsePhotos("3");
                                }
                                break;
                        }
                    }
                    break;
                case "ion-trash-b":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove image?", "Remove", "Remove", "Cancel", null);
                        if (result)
                        {
                            
                            ImageFiles3 = null;
                            Title3 = "ion-camera";
                        }
                    }
                    break;
            }
        }

        private async void SecondImage()
        {
            switch (Title2)
            {
                case "ion-camera":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("Choose", "Cancel", "OK", null, "Take a photo", "Browse photos");
                        switch (result)
                        {
                            case "Take a photo":
                                {
                                     await TakePhoto("2");
                                }
                                break;
                            case "Browse photos":
                                {
                                    await BrowsePhotos("2");
                                }
                                break;
                        }
                    }
                    break;
                case "ion-trash-b":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove image?", "Remove", "Remove", "Cancel", null);
                        if (result)
                        {
                           
                            Title2 = "ion-camera";
                            ImageFiles2 =null;
                        }
                    }
                    break;
            }

        }

        private async void FirstImage()
        {
            switch (Title1)
            {
                case "ion-camera":
                    {
                        var result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("Choose", "Cancel", "OK", null, "Take a photo", "Browse photos");
                        switch (result)
                        {
                            case "Take a photo":
                                {
                                     await TakePhoto("1");
                                }
                                break;
                            case "Browse photos":
                                {
                                     await BrowsePhotos("1");
                                }
                                break;
                        }
                    }
                    break;
                case "ion-trash-b":
                    {

                        var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove image?", "Remove", "Remove", "Cancel", null);
                        if (result)
                        {
                            
                            ImageFiles1 = null;
                            Title1 = "ion-camera";
                        }
                    }
                    break;
            }

        }
        private async Task BrowsePhotos(string Image)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Browsing", "You cannot browse photos", "OK");

                }
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    CompressionQuality = 50,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                if (file == null)
                    return;
                switch (Image)
                {
                    case "1":
                        {
                            Title1 = "ion-trash-b";
                            File1 = new StreamContent(file.GetStream());
                            File1.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                FileName = Guid.NewGuid().ToString(),
                                Name = "image1"
                            };
                            File1.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            ImageFiles1 = new MultipartFormDataContent();
                            ImageFiles1.Add(File1);

                        }
                        break;
                    case "2":
                        {
                            Title2 = "ion-trash-b";
                            File2 = new StreamContent(file.GetStream());
                            File2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                FileName = Guid.NewGuid().ToString(),
                                Name = "imag2"
                            };
                            File2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            ImageFiles2 = new MultipartFormDataContent();
                            ImageFiles2.Add(File2);

                        }
                        break;
                    case "3":
                        {
                            Title3 = "ion-trash-b";
                            File3 = new StreamContent(file.GetStream());
                            File3.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                FileName = Guid.NewGuid().ToString(),
                                Name = "imag2"
                            };
                            File3.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            ImageFiles3 = new MultipartFormDataContent();
                            ImageFiles3.Add(File3);
                        }
                        break;
                }
                if(file != null)
                   file.Dispose();
            }
            catch (Exception)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Gallery browsing failed.");
            }
        }
        private async Task TakePhoto(string Image)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No camera available", "No Camera", "OK");
                    
                }
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Name = "fix_" + Guid.NewGuid().ToString(),
                    AllowCropping = true,
                    CompressionQuality = 50,
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null)
                    return;
                switch (Image)
                    {
                        case "1":
                            {
                                Title1 = "ion-trash-b";
                                File1 = new StreamContent(file.GetStream());
                                File1.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Path),
                                    Name = "image1"
                                };
                                File1.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                                ImageFiles1 = new MultipartFormDataContent();
                                ImageFiles1.Add(File1);
                                
                            }
                            break;
                        case "2":
                            {
                                Title2 = "ion-trash-b";
                                File2 = new StreamContent(file.GetStream());
                                File2.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Path),
                                    Name = "image2"
                                };
                                File2.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                                ImageFiles2 = new MultipartFormDataContent();
                                ImageFiles2.Add(File2);
                               
                            }
                            break;
                        case "3":
                            {
                                Title3 = "ion-trash-b";
                                File3 = new StreamContent(file.GetStream());
                                File3.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                                {
                                    FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.Path),
                                    Name = "image2"
                                };
                                File3.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                                ImageFiles3 = new MultipartFormDataContent();
                                ImageFiles3.Add(File3);
                                
                            }
                            break;
                    }
                if(file != null)
                file.Dispose();
            }
            catch (Exception ex)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
        private async Task GetCategories()
        {
            try
            {

                var categories = await ServerPath.Path
                    .AppendPathSegment("/api/categories/getcategories/" + ClientModule.Tenant)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonAsync<List<Category>>();
                CategoryList = new List<Category>(categories);

            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No category found.");
            }

        }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Logging Issue");


                if (ImageFiles1 != null)
                {

                    var image = await ServerPath.Path
                        .AppendPathSegment("/api/issues/postimage").PostAsync(ImageFiles1).ReceiveString();
                    if (image != null)
                    {

                        _path1 = "https://fixmyplace.blob.core.windows.net/files/" + image.Replace(@"""", "").Trim();
                    }
                }
                if (ImageFiles2 != null)
                {


                    var image = await ServerPath.Path
                        .AppendPathSegment("/api/issues/postimage").PostAsync(ImageFiles2).ReceiveString();
                    if (image != null)
                    {
                        _path2 = "https://fixmyplace.blob.core.windows.net/files/" + image.Replace(@"""", "").Trim();
                    }
                }
                if (ImageFiles3 != null)
                {
                    var image = await ServerPath.Path
                        .AppendPathSegment("/api/issues/postimage").PostAsync(ImageFiles3).ReceiveString();
                    if (image != null)
                    {
                        _path3 = "https://fixmyplace.blob.core.windows.net/files/" + image.Replace(@"""", "").Trim();
                    }
                }
                //if(Position != null)
                //{
                //    Longitude = Position.Value.Longitude;
                //    Latitude = Position.Value.Latitude;
                //}

                var post = await ServerPath.Path
                    .AppendPathSegment("/api/issues/addissuemobile/" + TenantName).PostJsonAsync(new
                {
                    Title,
                    Description,
                    IsResolved = false,
                    Address,
                    Category.CategoryId,
                    JobPerformed = "",
                    PostedOn = DateTime.Now,
                    Status = "Ack",
                    Customer.CustomerId,
                    ImageUrl1 = _path1,
                    ImageUrl2 = _path2,
                    ImageUrl3 = _path3,
                    Longitude,
                    Latitude
                });
                if (post != null)
                {

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Title = string.Empty;
                    Description = string.Empty;
                    ImageFiles1 = null;
                    ImageFiles2 = null;
                    ImageFiles3 = null;
                    Latitude = null;
                    Longitude = null;
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Issue has been submitted");
                    NavigationParameters para = new NavigationParameters();
                    para.Add("tenantName", ClientModule.Tenant);
                    para.Add("customer", Customer);
                    await _navigationService.NavigateAsync("ClientIssuesPage", para);

                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Customer = parameters["customer"] as Customer;
                Address = Customer.Unit;
                TenantName = parameters["tenantName"].ToString();
                if(Customer.Unit != null)
                {
                    Enabled = false;
                }
                else
                {
                    Enabled = true;
                }
                
            }
        }
    }
}
