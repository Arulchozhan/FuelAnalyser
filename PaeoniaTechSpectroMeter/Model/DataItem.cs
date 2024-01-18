using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaeoniaTechSpectroMeter.Model
{
    public class DataItem : INotifyPropertyChanged
    {
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public string PassNo { get; set; }
        public string Operator { get; set; }
        public string AnalysisType { get; set; }
        public string SampleType { get; set; }
        public double? Ethanol { get; set; }
        public double? Denaturant { get; set; }
        public double? Methanol { get; set; }
        public double? Water { get; set; }

        //public bool IsSelected { get; set; }

        public int? Batch { get; set; }

        public string FormattedTimestamp => Timestamp.ToString("dd/MM/yyyy HH:mm:ss");

        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
