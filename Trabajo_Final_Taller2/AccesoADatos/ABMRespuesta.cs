﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Entidades;

namespace AccesoADatos
{
    public class ABMRespuesta
    {
        /// <summary>
        /// 
        /// </summary>
        void CargarRespuesta(int idUser, int idPregunta, string titulo, string descripcion, string urlImg, DateTime fecha, int likes)
        {
            try
            {
                //Hacer conexion a la base de datos
                Conexion_Desconexion.Connection();
                string query = @"INSERT INTO Respuestas(id_user,id_pregunta,titulo,descripcion,url_imagen,fecha,likes) VALUES(@id_user,@id_pregunta,@titulo,@descripcion,@url_imagen,@fecha,@likes)";
                SqlCommand command = new SqlCommand(query, Conexion_Desconexion.Con);
                /*NOTA: placeholders.*/
                command.Parameters.AddWithValue("@id_user", idUser);
                command.Parameters.AddWithValue("@id_pregunta",idPregunta);
                command.Parameters.AddWithValue("@titulo", titulo);
                command.Parameters.AddWithValue("@descripcion", descripcion);
                command.Parameters.AddWithValue("@url_imagen", urlImg);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@likes", likes);
                command.ExecuteNonQuery();
                //Desconectar
                Conexion_Desconexion.Desconnect();
            }
            catch (Exception ex)
            {
                //Nloggear
                throw;
            }
        }

        /// <summary>
        /// Selecciona las respuestas de la pregunta especificada en la base de datos
        /// </summary>
        /// <param name="preg"></param>
        /// <returns></returns>
        public static List<Respuesta> GetRespuestas(Pregunta preg)
        {
            List<Respuesta> respuestas = new List<Respuesta>();

            // Hacer consulta en la base de datos para obtener las respuestas de la pregunta

            return respuestas;
        }
        
        /// <summary>
        /// Carga el usuario al objeto respuesta
        /// </summary>
        public static void CargarUser(Respuesta resp)
        {
            resp.UserRespuesta = ABMUsuario.GetUsuario(resp.IdUserResp);
        }
    }
}