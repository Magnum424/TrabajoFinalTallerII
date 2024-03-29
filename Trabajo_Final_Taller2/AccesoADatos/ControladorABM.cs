﻿using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccesoADatos
{
    /// <summary>
    /// Clase helper para manejar el acceso a la base de datos
    /// </summary>
    static public class ControladorABM
    {

        /// <summary>
        /// Carga la lista de preguntas al objeto usuario 
        /// </summary>
        /// <param name="user"></param>
        static public void CargarListaPreguntas(Usuario user)
        {
            user.Preguntas = ABMPregunta.GetPreguntas(user.IdUsuario);

            foreach (Pregunta p in user.Preguntas)
            {
                string estadoInicial = p.Estado;
                p.ChequearEstado();
                string estadoFinal = p.Estado;

                if (estadoInicial != estadoFinal)
                {
                    // Se debe cambiar el estado de la pregunta en la base de datos
                    ABMPregunta.ActualizarEstado(p.IdPregunta, estadoFinal);
                }
            }

            // Asignar la referencia al user para cada una de sus preguntas
            // y cargar sus listas de respuestas
            foreach (Pregunta p in user.Preguntas)
            {
                p.UserPregunta = user;
                CargarListaRespuestas(p);
            }
        }

        /// <summary>
        /// Carga la lista de notificaciones al objeto usuario 
        /// </summary>
        /// <param name="user"></param>
        static public void CargarListaNotificaciones(Usuario user)
        {
            user.Notificaciones = ABMNotificacion.GetNotificaciones(user.IdUsuario);

            // Asignar la referencia al user y las preguntas para cada una de sus notificaciones
            foreach (Notificacion n in user.Notificaciones)
            {
                n.UsuarioPregunta = user;
                n.PreguntaNotif = user.Preguntas.Find(p => p.IdPregunta == n.IdPregunta);
            }
        }


        /// <summary>
        /// Carga la lista de respuestas al objeto pregunta 
        /// </summary>
        /// <param name="preg"></param>
        static public void CargarListaRespuestas(Pregunta preg)
        {
            preg.Respuestas = ABMRespuesta.GetRespuestas(preg.IdPregunta);

            // Asignar la referencia a la pregunta para cada una de sus respuestas
            foreach (Respuesta r in preg.Respuestas)
            {
                r.PregRespuesta = preg;
                r.UserRespuesta = ABMUsuario.GetUsuario(r.IdUserResp); // Cargar usuario que hizo la respuesta
                r.IdsUsuariosLike = ABMRespuesta.GetIdsUsuariosLike(r.IdRespuesta);
            }

            // Asignar la referencia a la solucion, si es que tiene solucion
            if (preg.EstaSolucionada())
            {
                preg.Solucion = preg.Respuestas.Find(r => r.IdRespuesta == preg.IdSolucion);
            }
        }


        /// <summary>
        /// Carga la referencia a usuario al objeto respuesta
        /// </summary>
        public static void CargarUser(Respuesta resp)
        {
            resp.UserRespuesta = ABMUsuario.GetUsuario(resp.IdUserResp);
        }


        /// <summary>
        /// Devuelve un usuario con datos y listas cargadas, 
        /// o null si la contraseña y el email no coinciden, 
        /// o no existe el usuario
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Usuario LoguearUsuario(string email, string password)
        {
            // Chequear si existe el usuario en la base de datos
            if (ABMUsuario.ExisteUser(email) == false)
            {
                return null;
            }
            else
            {
                Usuario user = ABMUsuario.GetUsuario(email);

                // Chequear si coincide la contraseña del usuario
                if (user.Password != password)
                {
                    return null;
                }
                else
                {
                    CargarListaPreguntas(user);
                    CargarListaNotificaciones(user);
                    return user;
                }
            }
        }

        /// <summary>
        /// Crea una nueva pregunta con imagen en la base de datos y recarga la lista de preguntas del usuario
        /// </summary>
        /// <param name="userPregunta"></param>
        /// <param name="tituloPreg"></param>
        /// <param name="descripcionPreg"></param>
        /// <param name="urlImagen"></param>
        public static void HacerPregunta(Usuario userPregunta, string tituloPreg, string descripcionPreg, string urlImagen)
        {
            // Alta en base de datos
            ABMPregunta.AltaPregunta(userPregunta.IdUsuario, tituloPreg, descripcionPreg, urlImagen);

            // Recarga la lista de preguntas
            CargarListaPreguntas(userPregunta);
        }

        /// <summary>
        /// Crea una nueva pregunta sin imagen en la base de datos y recarga la lista de preguntas del usuario
        /// </summary>
        /// <param name="userPregunta"></param>
        /// <param name="tituloPreg"></param>
        /// <param name="descripcionPreg"></param>
        public static void HacerPregunta(Usuario userPregunta, string tituloPreg, string descripcionPreg)
        {
            // Alta en base de datos
            ABMPregunta.AltaPregunta(userPregunta.IdUsuario, tituloPreg, descripcionPreg);

            // Recarga la lista de preguntas
            CargarListaPreguntas(userPregunta);
        }

        /// <summary>
        /// Crea una nueva respuesta con imagen en la base de datos y una notificacion si es necesaria, 
        /// y recarga la lista de respuestas de la pregunta
        /// </summary>
        /// <param name="userRespuesta"></param>
        /// <param name="preg"></param>
        /// <param name="tituloResp"></param>
        /// <param name="descripcionResp"></param>
        /// <param name="urlImg"></param>
        public static void ResponderPregunta(Usuario userRespuesta, Pregunta preg, string tituloResp, string descripcionResp, string urlImg)
        {
            ABMRespuesta.AltaRespuesta(userRespuesta.IdUsuario, preg.IdPregunta, tituloResp, descripcionResp, urlImg);
            CargarListaRespuestas(preg);


            if (preg.EmiteNotificacion() == true)
            {
                // Si responde a su propia pregunta no emite notificacion
                if (userRespuesta.IdUsuario != preg.IdUserPregunta)
                {
                    ABMNotificacion.AltaNotificacion(preg.IdUserPregunta, preg.IdPregunta);
                }
            }
        }

        /// <summary>
        /// Crea una nueva respuesta sin imagen en la base de datos y una notificacion si es necesaria, 
        /// y recarga la lista de respuestas de la pregunta
        /// </summary>
        /// <param name="userRespuesta"></param>
        /// <param name="preg"></param>
        /// <param name="tituloResp"></param>
        /// <param name="descripcionResp"></param>
        public static void ResponderPregunta(Usuario userRespuesta, Pregunta preg, string tituloResp, string descripcionResp)
        {
            if (preg.AdmiteRespuesta())
            {
                ABMRespuesta.AltaRespuesta(userRespuesta.IdUsuario, preg.IdPregunta, tituloResp, descripcionResp);
                CargarListaRespuestas(preg);

                if (preg.EmiteNotificacion() == true)
                {
                    // Si responde a su propia pregunta no emite notificacion
                    if (userRespuesta.IdUsuario != preg.IdUserPregunta)
                    {
                        ABMNotificacion.AltaNotificacion(preg.IdUserPregunta, preg.IdPregunta);
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza los likes de una respuesta en la base de datos y en el objeto
        /// y en la respuesta
        /// </summary>
        /// <param name="userLike"></param>
        /// <param name="resp"></param>
        public static void DarLike(Usuario userLike, Respuesta resp)
        {
            if (resp.DioLike(userLike) == false)
            {
                ABMRespuesta.AltaLike(resp.IdRespuesta, userLike.IdUsuario);
                resp.IdsUsuariosLike.Add(userLike.IdUsuario);
            }
        }

        /// <summary>
        /// Actualiza los likes de una respuesta en la base de datos y en el objeto
        /// y en la respuesta
        /// </summary>
        /// <param name="userLike"></param>
        /// <param name="resp"></param>
        public static void DarDisike(Usuario userLike, Respuesta resp)
        {
            if (resp.DioLike(userLike))
            {
                ABMRespuesta.BajaLike(resp.IdRespuesta, userLike.IdUsuario);
                resp.IdsUsuariosLike.Remove(userLike.IdUsuario);
            }
        }

        /// <summary>
        /// Elimina una pregunta de la base de datos
        /// y la quita de la lista de preguntas del usuario
        /// </summary>
        /// <param name="preg"></param>
        public static void EliminarPregunta(Pregunta preg)
        {
            ABMPregunta.BajaPregunta(preg.IdPregunta);
            preg.UserPregunta.Preguntas.Remove(preg);
        }

        /// <summary>
        /// Elimina una respuesta de la base de datos
        /// y la quita de la lista de respuestas de la pregunta
        /// </summary>
        /// <param name="resp"></param>
        public static void EliminarRespuesta(Respuesta resp)
        {
            ABMRespuesta.BajaRespuesta(resp.IdRespuesta);
            resp.PregRespuesta.Respuestas.Remove(resp);
        }

        /// <summary>
        /// Elimina una notificacion de la base de datos 
        /// y la quita de la lista de notificaciones del usuario
        /// </summary>
        /// <param name="notif"></param>
        public static void EliminarNotificacion(Notificacion notif)
        {
            ABMNotificacion.BajaNotificacion(notif.IdNotificacion);
            notif.UsuarioPregunta.Notificaciones.Remove(notif);
        }


        /// <summary>
        /// Elimina al usuario de la base de datos
        /// </summary>
        /// <param name="user"></param>
        public static void EliminarCuenta(Usuario user)
        {
            ABMUsuario.BajaUsuario(user.IdUsuario);
        }

        /// <summary>
        /// Actualiza la base de datos en la pregunta
        /// con la respuesta seleccionada
        /// y vincula la referencia a la solucion en la pregunta
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="preg"></param>
        public static void SolucionarPregunta(Respuesta resp, Pregunta preg)
        {
            if (preg.EstaSolucionada() == false)
            {
                ABMPregunta.UpdateSolucionPregunta(preg.IdPregunta, resp.IdRespuesta);
                preg.Solucion = resp;
                preg.IdSolucion = resp.IdRespuesta;
                preg.Estado = "Solucionada";
            }
        }

        /// <summary>
        /// Retorna una lista de preguntas con sus listas de respuestas cargadas
        /// </summary>
        /// <returns></returns>
        public static List<Pregunta> ObtenerTodasLasPreguntas()
        {
            List<Pregunta> todasLasPreguntas = ABMPregunta.GetPreguntas();


            foreach (Pregunta p in todasLasPreguntas)
            {
                string estadoInicial = p.Estado;
                p.ChequearEstado();
                string estadoFinal = p.Estado;

                if (estadoInicial != estadoFinal)
                {
                    // Se debe cambiar el estado de la pregunta en la base de datos
                    ABMPregunta.ActualizarEstado(p.IdPregunta, estadoFinal);
                }
            }
            todasLasPreguntas.ForEach(p => p.UserPregunta = ABMUsuario.GetUsuario(p.IdUserPregunta));
            todasLasPreguntas.ForEach(p => CargarListaRespuestas(p));
            return todasLasPreguntas;
        }
        public static void cambiarPass(Usuario user)
        {
            ABMUsuario.modificarAtributo(user.IdUsuario, "test");
        }
    }
}
