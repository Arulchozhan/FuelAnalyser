using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using PaeoniaTechSpectroMeter.Data;
using Microsoft.Win32;
//using MVPManager;
using System.Windows;
using System.Diagnostics;
using Utilities;

namespace PaeoniaTechSpectroMeter.Model
{
  public  class RecipeManager : INotifyPropertyChanged //,IDispatcherUIDisplay
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        ControlRecipe controlRecipe = new ControlRecipe();
        public ControlRecipe ControlRecipe
        {
            get { return controlRecipe; }
            private set
            {
                controlRecipe = value;
                OnPropertyChanged("ControlRecipe");
            }
        }

        ObservableCollection<string> controlRecipeNameList = new ObservableCollection<string>();
        public ObservableCollection<string> ControlRecipeNameList
        {
            get { return controlRecipeNameList; }
        }

        string selectedControlRecipeName = null;
        public string SelectedControlRecipeName
        {
            get { return selectedControlRecipeName; }
            set
            {

                if (selectedControlRecipeName == value)
                    return;

                selectedControlRecipeName = value;
                OnPropertyChanged("SelectedControlRecipeName");
            }
        }

        public void CollectAvailableControlRecipes()
        {
            SelectedControlRecipeName = "";
            List<string> tempFileNames = new List<string>();
            string[] files = Directory.GetFiles(SystemPath.GetControlRecipePath, "*." + SystemPath.ControlExt);
            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo info = new FileInfo(files[i]);
                    tempFileNames.Add(info.Name.Replace("." + SystemPath.ControlExt, ""));
                }
            }

            UpdateControlRecipeList(tempFileNames);
            if (isControlRecipeLoaded)
            {
                for (int i = 0; i < ControlRecipeNameList.Count; i++)
                {
                    if (loadedControlRecipe != ControlRecipeNameList[i]) continue;

                    SelectedControlRecipeName = loadedControlRecipe;
                    break;
                }
            }
            else if (controlRecipeNameList.Count == 0)
            {
                string path = Path.Combine(SystemPath.GetControlRecipePath, "Default" + "." + SystemPath.ControlExt);
                SaveControlRecipe(path);
            }
        }

        public void UpdateControlRecipeList(List<string> fileNameList)
        {
            bool tempUpdateData = false;
            if (ControlRecipeNameList.Count != fileNameList.Count)
            {
                tempUpdateData = true;
            }

            if (!tempUpdateData)
            {
                for (int i = 0; i < fileNameList.Count; i++)
                {
                    if (fileNameList[i] != controlRecipeNameList[i])
                    {
                        tempUpdateData = true;
                        break;
                    }
                }
            }
            if (tempUpdateData)
            {
                controlRecipeNameList.Clear();
                for (int i = 0; i < fileNameList.Count; i++)
                {
                    controlRecipeNameList.Add(fileNameList[i]);
                }
            }
        }
        public void LoadLastUsedControlRecipe()
        {
            if (App.MainMngr.AppConfig.LastControlRecipeName != "")
            {
                LoadControlRecipe(App.MainMngr.AppConfig.LastControlRecipeName);
            }



        }
       
        public void SaveControlRecipe()
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = SystemPath.GetControlRecipePath;
            if (isControlRecipeLoaded)
                saveFileDialog.FileName = loadedControlRecipe;
            saveFileDialog.Filter = "Recipe File (*." + SystemPath.ControlExt + ")|*." + SystemPath.ControlExt + "|All Files (*.*)|*.*";
            if ((bool)saveFileDialog.ShowDialog())
                SaveControlRecipe(saveFileDialog.FileName);
        }

        public void SaveControlRecipe(string fullPath)
        {
            try
            {

                IsRecipeSavingAndLoading = true;

                FileInfo fileInfo = new FileInfo(fullPath);

                string str = "Saving Control Recipe [" + fileInfo.Name + "]";
                App.MainMngr.McStatusMonitor.SetOverallStatus(str);
                App.ErrorEventMgr.ProcessEvent(str);

                bool success = Serializer.SaveXml(typeof(ControlRecipe), controlRecipe, fullPath);
                if (!success)
                    throw new Exception(Serializer.LastError);


                LoadedControlRecipe = fileInfo.Name.Replace("." + SystemPath.ControlExt, "");
                IsControlRecipeLoaded = true;

                CollectAvailableControlRecipes();

                controlRecipe.WriteDownObjectID(" Saved ");

                str = "Saved Control Recipe [" + fileInfo.Name + "]";
                App.MainMngr.McStatusMonitor.SetOverallStatus(str);
                App.ErrorEventMgr.ProcessEvent(str);
            }
            catch (Exception ex)
            {
                App.MainMngr.McStatusMonitor.SetOverallStatus(ex.Message, MCPNet.MachineMonitor.StatusInfoType.Error);
                Debug.WriteLine(Serializer.LastError);
                Debug.WriteLine(Serializer.LastErrorDetail);
                MessageBox.Show(ex.Message);
                App.ErrorEventMgr.ProcessError("Control Save Error", ex.ToString());
            }
            finally
            {
                IsRecipeSavingAndLoading = false;
            }
        }

        bool isRecipeSavingAndLoading = false;
        public bool IsRecipeSavingAndLoading
        {
            get { return isRecipeSavingAndLoading; }
            set
            {
                isRecipeSavingAndLoading = value;
                OnPropertyChanged("IsRecipeSavingAndLoading");
            }
        }

        public bool LoadControlRecipe(string fileName)
        {
            bool loadingOK = false;
            do
            {
                try
                {
                    if (LoadedControlRecipe == fileName) { loadingOK = true; break; }

                    string str = "Loading Control Recipe [" + fileName + "]";
                    App.MainMngr.McStatusMonitor.SetOverallStatus(str);
                    App.ErrorEventMgr.ProcessEvent(str);

                    IsRecipeSavingAndLoading = true;

                    string fullPath = Path.Combine(SystemPath.GetControlRecipePath, fileName + "." + SystemPath.ControlExt);
                    if (File.Exists(fullPath))
                    {
                        ControlRecipe tempRec = (ControlRecipe)Serializer.LoadXml(typeof(ControlRecipe), fullPath);
                        if (tempRec != null)
                        {
                            controlRecipe = tempRec;
                        }
                        else
                        {
                            App.ErrorEventMgr.ProcessError("Control Recipe Load Error" + Environment.NewLine +
                                                           fullPath +
                                                           Environment.NewLine +
                                                           Serializer.LastError,
                                                           Serializer.LastErrorDetail);

                            throw new Exception("Control Recipe Load Error");
                        }

                        FileInfo fileInfo = new FileInfo(fullPath);
                        IsControlRecipeLoaded = true;
                        LoadedControlRecipe = fileInfo.Name.Replace("." + SystemPath.ControlExt, "");


                        CollectAvailableControlRecipes();
                        controlRecipe.InitialOnLoaded();

                        str = "Control Recipe Loaded [" + fileName + "]";
                        App.MainMngr.McStatusMonitor.SetOverallStatus(str);
                        App.ErrorEventMgr.ProcessEvent(str);

                        loadingOK = true;
                    }
                    OnPropertyChanged("ControlRecipe");
                    controlRecipe.WriteDownObjectID(" Loaded ");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    App.ErrorEventMgr.ProcessError("Control Recipe Load Error", ex.ToString());
                }
                finally
                {
                    IsRecipeSavingAndLoading = false;
                }
            } while (false);

            return loadingOK;
        }
        static string UntitledRecipeName = "untitle";

        bool isControlRecipeLoaded = false;
        public bool IsControlRecipeLoaded
        {
            get { return isControlRecipeLoaded; }
            private set
            {
                isControlRecipeLoaded = value;
                OnPropertyChanged("IsControlRecipeLoaded");
            }
        }
        string loadedControlRecipe = "";
        public string LoadedControlRecipe
        {
            get
            {
                return isControlRecipeLoaded ? loadedControlRecipe : UntitledRecipeName;
            }
            private set
            {

                loadedControlRecipe = value;
                OnPropertyChanged("LoadedControlRecipe");
            }
        }
    }
}
