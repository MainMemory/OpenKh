using Microsoft.Win32;
using OpenKh.Kh2;
using OpenKh.Tools.Kh2ObjectEditor.Classes;
using OpenKh.Tools.Kh2ObjectEditor.Modules.Motions;
using OpenKh.Tools.Kh2ObjectEditor.Services;
using OpenKh.Tools.Kh2ObjectEditor.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpenKh.Tools.Kh2ObjectEditor.Views
{
    public partial class ModuleMotions_Control : UserControl
    {
        public ModuleMotions_VM ThisVM { get; set; }
        public ModuleMotions_Control()
        {
            InitializeComponent();
            ThisVM = new ModuleMotions_VM();
            DataContext = ThisVM;
        }

        private void Button_ApplyFilters(object sender, System.Windows.RoutedEventArgs e)
        {
            ThisVM.applyFilters();
        }

        private void list_doubleCLick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MotionSelector_Wrapper item = (MotionSelector_Wrapper)(sender as ListView).SelectedItem;


            // Reaction Command
            if (item.Entry.Type == Bar.EntryType.Motionset)
            {
                System.Windows.Forms.MessageBox.Show("This is a Reaction Command", "Motion couldn't be loaded", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            // No motion
            else if(item.Entry.Type != Bar.EntryType.Anb)
            {
                System.Windows.Forms.MessageBox.Show("This is not a Motion or Reaction Command", "Motion couldn't be loaded", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            // Invalid
            if (item == null || item.Name.Contains("DUMM"))
            {
                return;
            }
            // Unnamed dummy
            if (item.Entry.Stream.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("This motion is a dummy (No data)", "Motion couldn't be loaded", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            try
            {
                App_Context.Instance.loadMotion(item.Index);
                //Mset_Service.Instance.loadMotion(item.Index);
                openMotionTabs(MsetService.Instance.LoadedMotion);
            }
            catch (System.Exception exc)
            {
                System.Windows.Forms.MessageBox.Show("There was an error", "Animation couldn't be loaded", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
        }

        private void openMotionTabs(AnimationBinary animBinary)
        {
            Frame_Metadata.Content = new MotionMetadata_Control(animBinary);
            Frame_Triggers.Content = new MotionTriggers_Control(animBinary);
        }
        public void Motion_Copy(object sender, RoutedEventArgs e)
        {
            if (MotionList.SelectedItem != null)
            {
                MotionSelector_Wrapper item = (MotionSelector_Wrapper)MotionList.SelectedItem;
                ThisVM.Motion_Copy(item.Index);
            }
        }
        public void Motion_Replace(object sender, RoutedEventArgs e)
        {
            if (MotionList.SelectedItem != null)
            {
                MotionSelector_Wrapper item = (MotionSelector_Wrapper)MotionList.SelectedItem;
                ThisVM.Motion_Replace(item.Index);
            }
        }
        public void Motion_Import(object sender, RoutedEventArgs e)
        {
            if (MotionList.SelectedItem != null)
            {
                Frame_Metadata.Content = new ContentControl();
                Frame_Triggers.Content = new ContentControl();

                MotionSelector_Wrapper item = (MotionSelector_Wrapper)MotionList.SelectedItem;

                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = false;
                    if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames != null && openFileDialog.FileNames.Length > 0)
                    {
                        string filePath = openFileDialog.FileNames[0];
                        if (filePath.ToLower().EndsWith(".fbx"))
                        {
                            ThisVM.Motion_Import(item.Index, filePath);
                        }
                    }

                    App_Context.Instance.loadMotion(item.Index);
                    openMotionTabs(MsetService.Instance.LoadedMotion);
                }
                catch (Exception exception)
                {
                    // Nothing planned for now
                }
            }
        }
        public void RC_Export(object sender, RoutedEventArgs e)
        {
            if (MotionList.SelectedItem != null)
            {
                MotionSelector_Wrapper item = (MotionSelector_Wrapper)MotionList.SelectedItem;

                if(item.Entry.Type != Bar.EntryType.Motionset) {
                    System.Windows.Forms.MessageBox.Show("The selected entry is not a Moveset", "Can't export entry", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    System.Windows.Forms.SaveFileDialog sfd;
                    sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.Title = "Export Reaction Command";
                    sfd.FileName = item.Entry.Name + ".mset";
                    sfd.ShowDialog();
                    if (sfd.FileName != "")
                    {
                        MemoryStream memStream = (MemoryStream)item.Entry.Stream;
                        memStream.Position = 0;
                        File.WriteAllBytes(sfd.FileName, memStream.ToArray());
                        item.Entry.Stream.Position = 0;
                    }
                }
                catch (Exception exc) { }

                item.Entry.Stream.Position = 0;
            }
        }

        private void Button_TEST(object sender, System.Windows.RoutedEventArgs e)
        {
            ThisVM.TestMsetIngame();
        }
    }
}
