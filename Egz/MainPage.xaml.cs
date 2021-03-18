using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Egz
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    /// 
    [Serializable]
    public class Zapominanie
    {
        public string Text;
    }
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ZametcaName();
        }

        void ZametcaName()
        {
            DirectoryInfo dir = new DirectoryInfo(ApplicationData.Current.LocalFolder.Path);
            foreach (var item in dir.GetFiles())
            {
                Item_zametca.Items.Add((item.Name).Remove(item.Name.Length - 4, 4));
                Item_vivod_text.Items.Add((item.Name).Remove(item.Name.Length - 4, 4));
            }
        }

        private async void dobav_Click(object sender, RoutedEventArgs e)
        {
            if (NameFile.Text.ToString() == "")
            {
                MessageDialog yy = new MessageDialog("Введите имя файла");
                await yy.ShowAsync();
            }
            else
            {
                List<Zapominanie> list = new List<Zapominanie>();
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync(NameFile.Text+".xml", CreationCollisionOption.OpenIfExists);
                XmlSerializer xml = new XmlSerializer(typeof(List<Zapominanie>));
                Stream stream = await file.OpenStreamForReadAsync();
                try
                {
                    list = (List<Zapominanie>)xml.Deserialize(stream);
                }
                catch
                {
                    list = new List<Zapominanie>();

                }
                stream.Close();
                stream = await file.OpenStreamForWriteAsync();
                list.Add(new Zapominanie()
                {
                    Text = Text_zametca.Text.ToString()
                });
                xml.Serialize(stream, list);
                stream.Close();
                Item_zametca.Items.Clear();
                Item_vivod_text.Items.Clear();
                ZametcaName();
            }
        }

        private async void Delit_Click(object sender, RoutedEventArgs e)
        {
            if (Item_zametca.SelectedItems.Count != 0)
            {
                try
                {
                    File.Delete(ApplicationData.Current.LocalFolder.Path + "\\" + Item_zametca.SelectedItem.ToString() + ".xml");
                    Item_zametca.Items.Clear();
                    Item_vivod_text.Items.Clear();
                    ZametcaName();
                    Text_zametca.Text = "";
                }
                catch { }
            }
            else
            {
                MessageDialog d = new MessageDialog("Вы не выбрали заметку для удаления");
                await d.ShowAsync();
            }
        }

        private async void Item_vivod_text_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Text_zametca.Text = "";
            List<Zapominanie> list1 = new List<Zapominanie>();
            StorageFolder folder1 = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder1.CreateFileAsync(Item_vivod_text.SelectedItem.ToString() + ".xml", CreationCollisionOption.OpenIfExists);
            XmlSerializer xml1 = new XmlSerializer(typeof(List<Zapominanie>));
            Stream stream1 = await file.OpenStreamForReadAsync();
            try
            {
                list1 = (List<Zapominanie>)xml1.Deserialize(stream1);
            }
            catch
            {
                list1 = new List<Zapominanie>();

            }
            stream1.Close();
            for (int i = 0; i < list1.Count; i++)
            {
                Text_zametca.Text = list1[i].Text;
            }
        }
    }
}
