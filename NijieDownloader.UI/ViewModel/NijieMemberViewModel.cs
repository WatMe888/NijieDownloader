﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NijieDownloader.Library.Model;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace NijieDownloader.UI.ViewModel
{
    public class NijieMemberViewModel : INotifyPropertyChanged
    {
        private NijieMember _member;
        public NijieMember Member
        {
            get { return _member; }
            private set
            {
                _member = value;
                onPropertyChanged("Member");
            }
        }

        private BitmapImage _avatarImage;
        public BitmapImage AvatarImage
        {
            get
            {
                if (_avatarImage == null)
                {
                    var loading = new BitmapImage(new Uri("pack://application:,,,/Resources/no_avatar.jpg"));
                    loading.Freeze();
                    if (Member != null)
                    {
                        MainWindow.LoadImage(Member.AvatarUrl, Member.MemberUrl,
                            new Action<BitmapImage, string>((image, status) =>
                            {
                                this.AvatarImage = null;
                                this.AvatarImage = image;
                            }
                        ));
                    }
                    _avatarImage = loading;
                }
                return _avatarImage;
            }
            set
            {
                _avatarImage = value;
                onPropertyChanged("AvatarImage");
            }
        }

        private ObservableCollection<NijieImageViewModel> _images;
        public ObservableCollection<NijieImageViewModel> Images
        {
            get
            {
                return _images;
            }
            set
            {
                _images = value;
                onPropertyChanged("Images");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public NijieMemberViewModel(NijieMember member)
        {
            this.Member = member;
            if (member != null && member.Images != null)
            {
                _images = new ObservableCollection<NijieImageViewModel>();
                foreach (var image in member.Images)
                {
                    var temp = new NijieImageViewModel(image);
                    _images.Add(temp);
                }
            }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                onPropertyChanged("Status");
            }
        }
    }
}