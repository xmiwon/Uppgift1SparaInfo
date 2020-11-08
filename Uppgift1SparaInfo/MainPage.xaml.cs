using Newtonsoft.Json;
using SharedClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Uppgift1SparaInfo
{
    public sealed partial class MainPage : Page
    {
        StorageFolder storageFolder;
        StorageFile file;
        public MainPage()
        {
            this.InitializeComponent();
            
        }


        //Open file explorer, select a file and read the file - DONE! G Uppgift
        private async void Button_Click_4( object sender, RoutedEventArgs e )
        {
            //Skapar en ny instance av File opener explorer och att vyn ska vara i list mode. Sedan filtrerar bort alla filer som INTE är av typen json, csv, txt eller xml
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".csv");
            picker.FileTypeFilter.Add(".xml");
            picker.FileTypeFilter.Add(".txt");

            //Tillåter användaren att välja en fil. Sen läser den innehållet av filen och sparar i textContent
            StorageFile file = await picker.PickSingleFileAsync();
            //Om den får "tag" på en vald fil, gö de här:
            if (file != null)
            {          
                var textContent = await FileIO.ReadTextAsync(file);
                //Skapar en ny instance av en listvy där innehållet läggs till listan
                ListView messageList = new ListView();
                listOfData.Items.Add(textContent);
                boxContainer.Children.Add(messageList);
            }

        }








        //VG uppgift

        //Här skapas en fil med varierande filnamn baserad på vad fileName får in
        private async Task ConstructFileAsync( string fileName )
        {

            storageFolder = KnownFolders.DocumentsLibrary;
            await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

        }
        

        //parameter content varierar och är inte alltid av samma typ
        private async Task WriteToFileAsync( dynamic content, string fileName )
        {
         //Hämtar filen
            file = await storageFolder.GetFileAsync(fileName);
            //Om filens typ är json eller txt, avänd WriteTextAsync metoden. Om det är csv blir det WriteLinesAsync istället
            if(file.FileType == ".json" || file.FileType == ".txt")
            {
                await FileIO.WriteTextAsync(file, content);
            } else if (file.FileType == ".csv")
            {
                await FileIO.WriteLinesAsync(file, content);
            }
            
        }





        //Save to json
        private async void Button_Click_2( object sender, RoutedEventArgs e )
        {
            //Alla värden läggs till variablar från inputsfälterna
            string firstName = tbFirstName.Text;
            string lastName = tbLastName.Text;
            //När man knappar in i inputfältet blir det string så då behövs det konvertera om till en int
            int age = Convert.ToInt32(tbAge.Text);
            string city = tbAddress.Text;

          //Skapar en ny objekt med värden som läggs in och sedan sparar till en variabel
            var storeInFile = new Person(firstName, lastName, age, city);
            //Konverterar om storeInFile objekt till en json format
            var json = JsonConvert.SerializeObject(storeInFile);
            //Skapar en json fil och sedan skriver in innehållet
            await ConstructFileAsync("jsonFile.json");
            WriteToFileAsync(json, "jsonFile.json").GetAwaiter();
           
        }




        //Save to csv
        private async void Button_Click_3( object sender, RoutedEventArgs e )
        {
            
            string firstName = tbFirstName.Text;
            string lastName = tbLastName.Text;
            int age = Convert.ToInt32(tbAge.Text);
            string city = tbAddress.Text;


            var csvContent = $"{firstName},{lastName},{age},{city}";
             //Lägger in alla värden till en lista av typen string
             var lines = new List<string>() { csvContent };


            await ConstructFileAsync("csvFile.csv");
            WriteToFileAsync(lines, "csvFile.csv").GetAwaiter();
        }





        //Save to xml
        private async void Button_Click_5( object sender, RoutedEventArgs e )
        {
            //Gör en ny constructor
           var person = new Person();
           person.FirstName = tbFirstName.Text;
           person.LastName = tbLastName.Text;
           person.Age = Convert.ToInt32(tbAge.Text);
           person.City = tbAddress.Text;
            //Initierar xmlserializer som hämtar typen av nuvarande instance av person
            var serializer = new XmlSerializer(person.GetType());
          
            

        //storageFoler är lika med Documents mappen
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            //Skapar xml filen i Documents mappen med tillval som ersätter om det redan existerar som sedan sparar det i en file (StorageFile)
             StorageFile file = await storageFolder.CreateFileAsync("xmlFile.xml", CreationCollisionOption.ReplaceExisting);

            //Gör så att det går överföra bytes (read/write)
            Stream stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false);

            using (stream)
            {
                //serialize om hela objekten till en xml innehåll med alla taggar osv
                serializer.Serialize(stream, person);
            }
            //stänger stream
            stream.Close(); 
        }

        //Save to .txt
        private async void Button_Click( object sender, RoutedEventArgs e )
        {
            string firstName = tbFirstName.Text;
            string lastName = tbLastName.Text;
            int age = Convert.ToInt32(tbAge.Text);
            string city = tbAddress.Text;


            
            await ConstructFileAsync("txtFile.txt");
            WriteToFileAsync($"My name is {firstName} {lastName}. I am {age} and live in {city}", "txtFile.txt").GetAwaiter();
        }
    }
}
