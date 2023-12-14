﻿using PaeoniaTechSpectroMeter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaeoniaTechSpectroMeter.Model
{
    public class SelfDiagnostics : UserControl, INotifyPropertyChanged
    {


        // private double[] ftyAir;
        //private double[] firstAir;
        //private double[] currentAir;
        //private double[] ftyOff;
        //private double[] currentOff;
        //private double[] ftyDiff;
        List<double> ftyAir = new List<double>();
        List<double> firstAir = new List<double>();
        List<double> currentAir = new List<double>();
        List<double> ftyOff = new List<double>();
        List<double> currentOff = new List<double>();
        List<double> ftyDiff = new List<double>();

        private double[] currentBackgroundData;

        private double[] factoryBackgroundData;

        private double[] newBackgroundData;
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion



        CtrlSelfDiagnostics ctrlSelfDiagnostics;
        MainManager mmgr;

        public SelfDiagnostics(MainManager mmgr)
        {

            this.mmgr = mmgr;
            ctrlSelfDiagnostics = new CtrlSelfDiagnostics(mmgr);


        }

        public double[] GetCurrentBackgroundData()
        {
            string currentairPath = "C:\\FuelAnalyzer\\Currentair" + ".csv";
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";


            ReadCsv(currentairPath, out currentAir);
            ReadCsv(ftytairPath, out ftyAir);

            currentBackgroundData = currentAir.Zip(ftyAir, (x, y) => x - y).ToArray(); //using Linq method to 
            return currentBackgroundData;
            //Random random = new Random();
            // return Enumerable.Range(0, 128).Select(_ => (random.NextDouble() * 0.2) + 2.4 - 2.5).ToArray();
        }

        public double[] GetFactoryBackgroundData()
        {
            string firstairPath = "C:\\FuelAnalyzer\\Firstair" + ".csv";
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";


            ReadCsv(firstairPath, out firstAir);
            ReadCsv(ftytairPath, out ftyAir);

            factoryBackgroundData = firstAir.Zip(ftyAir, (x, y) => x - y).ToArray(); //using Linq method to 
            return factoryBackgroundData;
            //Random random = new Random();
            // return Enumerable.Range(0, 128).Select(_ => (random.NextDouble() * 0.2) + 2.4 - 2.5).ToArray();
        }

        //public double[] GetNewBackgroundData()
        //{

        //    return;

        //}
        // GetNewBackgroundData();

        private string ReadCsv(string filepath, out List<double> result)
        {
            string serr = "";

            result = new List<double>();
            result.Clear();
            StreamReader ControlPageReader = null;
            try
            {
                ControlPageReader = new StreamReader(filepath);
                string sDataLine = "";
                string[] sControlData = { "" };

                while (true)
                {
                    sDataLine = ControlPageReader.ReadLine();

                    if (sDataLine == null)
                        break;

                    sControlData = sDataLine.Split(',');

                    result.Add((Convert.ToDouble(sControlData[0])));

                }
                ControlPageReader.Dispose();
                ControlPageReader.Close();
                return serr;
            }
            catch (Exception ex)
            {
                serr = ex.Message;
                ControlPageReader.Dispose();
                ControlPageReader.Close();
                return serr;
            }


        }
        public void WriteCsv(string filepath, Double[] result, bool Isappend)
        {
            using (StreamWriter writer = new StreamWriter(filepath, Isappend))
            {
                foreach (double value in result)
                {
                    writer.WriteLine(value);
                }


            }

        }



        public double[] GetNewBackgroundData()
        {

            newBackgroundData = mmgr.ReadDetector.ReadPixelVolt(true);
            return newBackgroundData;
            //Random random = new Random();
            //return Enumerable.Range(0, 128).Select(_ => (random.NextDouble() * 0.2) + 2.6 - 2.5).ToArray();
        }


    }




}