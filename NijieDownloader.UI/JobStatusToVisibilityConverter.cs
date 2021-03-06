﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using NijieDownloader.UI.ViewModel;
using System.Windows;

namespace NijieDownloader.UI
{
    public class JobStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            JobStatus val = (JobStatus)value;
            
            if (targetType == typeof(Visibility))
            {
                if (val == JobStatus.Running || val == JobStatus.Canceling)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
