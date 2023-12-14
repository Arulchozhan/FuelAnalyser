using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using PaeoniaTechSpectroMeter.Model;

namespace PaeoniaTechSpectroMeter.Commands
{
  
public class MainManagerBasicCmd : ICommand
{
    MainManager mgr;
    Action act;
    string warning = "";
    static Window mainWinInts = null;
    public MainManagerBasicCmd(MainManager mgr, Action ac, MainWindow mainWinInts, string warning = "", bool notify = false)
    {
        this.mgr = mgr;
        act = ac;
        this.warning = warning;
        MainManagerBasicCmd.mainWinInts = mainWinInts;
    }

    public bool CanExecute(object parameter)
    {
        return mgr != null &&
               !mgr.MainSMC.IsBusy &&
               !mgr.McStatusMonitor.IsBusy;
    }

    public event System.EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        try
        {
            if (mgr != null && act != null)
            {
                if (warning != "")
                {
                    if (MessageBoxResult.Yes ==
                        MessageBox.Show(mainWinInts, warning, "Attentation!",
                                        MessageBoxButton.YesNo,
                                        MessageBoxImage.Question))
                        act();
                }
                else
                {
                    act();
                }

            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            Debug.WriteLine(ex.ToString());
        }
    }
}
}
