using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Markup;
using System.Xml;

namespace NoteAppOnWPF
{
    public partial class MainWindow : Window
    {
        private static string path = @"C:\NoteAppNotes";
        private static string LastNoteTitle = "";
        public MainWindow()
        {
            InitializeComponent();
            UpdateNoteList();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            LastNoteTitle = "";
            NoteTitle.Text = "New note title";
            NoteText.Text = "";
        }

        public void UpdateNoteList()
        {
            NoteTitle.Text = "";
            NoteText.Text = "";
            LastNoteTitle = "";

            var NotesCount = NoteList.Children.Count;

            for (int i = 1; i < NotesCount; i++)
                NoteList.Children.RemoveAt(1);

            var dirInfo = new DirectoryInfo(path);

            if (!dirInfo.Exists)
                dirInfo.Create();

            string[] files = Directory.GetFiles(path, "*.txt");
            DateTime[] filesTime = new DateTime[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                filesTime[i] = File.GetLastWriteTime($@"{files[i]}");

                var button = new Button();
                button.Height = 25;
                button.Content = (files[i].Remove(files[i].Length - 4)).Remove(0, path.Length + 1) + ", "+filesTime[i];
                button.HorizontalContentAlignment = HorizontalAlignment.Left;
                button.Name = ("b_"+(files[i].Remove(files[i].Length - 4)).Remove(0, path.Length + 1) + "NoteButton").Replace(' ', '_');
                button.MouseDoubleClick += NoteDoubleClick;
                NoteList.Children.Add(button);
            }

        }

        private void NoteDoubleClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int index = button.Content.ToString().IndexOf(',');
            NoteTitle.Text = (button.Content.ToString()).Remove(index);
            LastNoteTitle = (button.Content.ToString()).Remove(index);

            using (FileStream fs = File.OpenRead($@"{path}\{(button.Content.ToString()).Remove(index)}.txt"))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);

                var fsString = Encoding.UTF8.GetString(array);

                NoteText.Text = fsString;
                fs.Close();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if(LastNoteTitle != NoteTitle.Text)
            {
                File.Delete($@"{path}\{LastNoteTitle}.txt");
                LastNoteTitle = NoteTitle.Text;
            }

            using (FileStream fs = File.OpenWrite($@"{path}\{NoteTitle.Text}.txt"))
            {
                byte[] array = Encoding.UTF8.GetBytes(NoteText.Text);

                fs.Write(array, 0, array.Length);
                fs.Close();
            }
            UpdateNoteList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string[] files = Directory.GetFiles(path, "*.txt");

            if (files.Contains($@"{path}\{LastNoteTitle}.txt"))
                File.Delete($@"{path}\{LastNoteTitle}.txt");

            UpdateNoteList();
        }
    }
}
