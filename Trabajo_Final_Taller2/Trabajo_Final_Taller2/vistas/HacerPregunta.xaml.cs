﻿using System;
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
using System.Windows.Shapes;
using AccesoADatos;
using Microsoft.Win32;

namespace Trabajo_Final_Taller2.vistas
{
    /// <summary>
    /// Lógica de interacción para HacerPregunta.xaml
    /// </summary>
    public partial class HacerPregunta : Window
    {
        public HacerPregunta()
        {
            InitializeComponent();
        }
        string imagenFinal = null;
        private void preguntar_Click(object sender, RoutedEventArgs e)
        {
            //obtener id user
            int iduser = 1;
            string titulo = tituloPreg.Text;
            string descripcion = tituloPreg.Text;
            //aun no se 100% como se va a manejar la imagen asi que por el momento lo dejo como un string...
            string imagen = imagenFinal;
            if (imagen != null)
            {
                ABMPregunta.AltaPregunta(iduser, titulo, descripcion, imagen);
            }
            else
            {
                //ABMPregunta.AltaPregunta(iduser, titulo, descripcion);
            }
          
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tituloPreg_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        /// <summary>
        /// Lo que hace este metodo es, al hacer click se abre un cuadro para seleccionar imagen; se usan unos parametros para especificar 
        /// que tipo de imagenes se pueden subir
        /// despues se obtiene el nombre del archivo, la ruta + el nombre, a donde se quiere copiar el archivo y destFile que va a ser la nueva ruta + el nombre dek archivo
        /// luego se copia la imagen en la carpeta de imagenes del proyecto.
        /// </summary>
        private void subirImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Seleccionar imagen";
            op.Filter = "Tipos de archivo|*.jpg;*jpeg;*png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                //imgPhoto.Source = new BitmapImage(new Uri(op.FileName));
                string fileName = op.SafeFileName;
                string source = op.FileName;
                string target = @"..\..\..\img";
                string destFile = System.IO.Path.Combine(target, fileName);
                System.IO.File.Copy(source, destFile, true);
                //destFile es lo que hay que guardar en la DB
                imagenFinal = destFile;
            }
        }
}
}
