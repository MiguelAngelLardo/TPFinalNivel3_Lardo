using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.ComponentModel;

namespace negocio

{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()

        {
            List<Articulo> lista = new List<Articulo>(); //ok
            SqlConnection conexion = new SqlConnection(); //ok
            SqlCommand comando = new SqlCommand(); //ok
            SqlDataReader lector; //ok


            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=CATALOGO_DB; integrated security=true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "select Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdMarca, A.IdCategoria, A.Id from ARTICULOS A, CATEGORIAS C, MARCAS M where IdMarca = M.ID and IdCategoria = C.Id";
                comando.Connection = conexion;
                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.Id = (int)lector["Id"];
                    aux.Codigo = (string)lector["Codigo"];
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    if (!(lector["ImagenUrl"] is DBNull))//si no es NULO lo leo
                        aux.ImagenUrl = (string)lector["ImagenUrl"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.Id = (int)lector["IdCategoria"];
                    aux.Categoria.Descripcion = (string)lector["Categoria"];

                    aux.Marca = new Marca();
                    aux.Marca.Id = (int)lector["IdMarca"];
                    aux.Marca.descripcionMarca = (string)lector["Marca"];

                    if (!(lector["Precio"] is DBNull))
                        aux.Precio = Math.Round((Decimal)lector["Precio"], 2);

                    lista.Add(aux);
                }

                conexion.Close();
                return lista;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void agregar(Articulo nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into ARTICULOS (Codigo, Nombre, Descripcion, Precio, ImagenUrl, IdMarca, IdCategoria) values (@codi, @nomb, @descri, @preci,@imagUrl, @idMarc, @idCatec)");

                datos.setearParametro("@codi", nuevo.Codigo);
                datos.setearParametro("@nomb", nuevo.Nombre);
                datos.setearParametro("@descri", nuevo.Descripcion);
                datos.setearParametro("@preci", nuevo.Precio);
                datos.setearParametro("@imagUrl", nuevo.ImagenUrl);
                datos.setearParametro("@idMarc", nuevo.Marca.Id);
                datos.setearParametro("@idCatec", nuevo.Categoria.Id);              

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo arti)

        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update ARTICULOS set Codigo = @cod, Nombre = @nomb, Descripcion = @desc, IdMarca = @idMar, IdCategoria = @idCat, ImagenUrl = @image, Precio = @pre where Id = @id");
                //8 parametros a agregar
                datos.setearParametro("@cod", arti.Codigo);
                datos.setearParametro("@nomb", arti.Nombre);
                datos.setearParametro("@desc", arti.Descripcion);
                datos.setearParametro("@idMar", arti.Marca.Id);
                datos.setearParametro("@idCat", arti.Categoria.Id);
                datos.setearParametro("@image", arti.ImagenUrl);
                datos.setearParametro("@pre", arti.Precio);
                datos.setearParametro("@id", arti.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }


        }

        public void eliminar(int id)

    {
        try
        {
            AccesoDatos datos = new AccesoDatos();
            datos.setearConsulta("delete from articulos where id =@id");
            datos.setearParametro("@id", id);
            datos.ejecutarAccion();
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

        public List<Articulo> filtrar(string campo, string criterio, string filtro)
        {
          List<Articulo> lista = new List<Articulo>();
          AccesoDatos datos = new AccesoDatos();
          try
          {
             string consulta = "select Codigo, Nombre, A.Descripcion, ImagenUrl, Precio, C.Descripcion Categoria, M.Descripcion Marca, A.IdMarca, A.IdCategoria, A.Id from ARTICULOS A, CATEGORIAS C, MARCAS M where IdMarca = M.ID and IdCategoria = C.Id And ";
              if(campo == "Nombre")
              {
                 switch (criterio)
                 {
                     case "Comienza con":
                         consulta += "Nombre like '" + filtro + "%' ";
                         break;
                     case "Termina con":
                         consulta += "Nombre like '%" + filtro + "'";
                         break;
                     default:
                         consulta += "Nombre like '%" + filtro + "%'";
                         break;
                 }
              }
              else if (campo == "Descripción")
              {
                  switch (criterio)
                  {
                      case "Comienza con":
                          consulta += "A.Descripcion like '" + filtro + "%' ";
                          break;
                      case "Termina con":
                          consulta += "A.Descripcion like '%" + filtro + "'";
                          break;
                      default:
                            consulta += "A.Descripcion like '%" + filtro + "%'";
                          break;
                    }
              }
              else
              {
                  switch (criterio)
                  {
                      case "Comienza con":
                          consulta += "M.Descripcion like '" + filtro + "%' ";
                          break;
                      case "Termina con":
                          consulta += "M.Descripcion like '%" + filtro + "'";
                          break;
                      default:
                          consulta += "M.Descripcion like '%" + filtro + "%'";
                          break;
                  }
              }


             datos.setearConsulta(consulta);
             datos.ejecutarLectura();
             while (datos.Lector.Read())
             {

                Articulo aux = new Articulo();
                aux.Id = (int)datos.Lector["Id"];
                aux.Codigo = (string)datos.Lector["Codigo"];
                aux.Nombre = (string)datos.Lector["Nombre"];
                aux.Descripcion = (string)datos.Lector["Descripcion"];

                if (!(datos.Lector["ImagenUrl"] is DBNull))//si no es NULO lo leo
                    aux.ImagenUrl = (string)datos.Lector["ImagenUrl"];

                aux.Categoria = new Categoria();
                aux.Categoria.Id = (int)datos.Lector["IdCategoria"];
                aux.Categoria.Descripcion = (string)datos.Lector["Categoria"];

                aux.Marca = new Marca();
                aux.Marca.Id = (int)datos.Lector["IdMarca"];
                aux.Marca.descripcionMarca = (string)datos.Lector["Marca"];

                if (!(datos.Lector["Precio"] is DBNull))
                    aux.Precio = Math.Round((Decimal)datos.Lector["Precio"], 2);

                lista.Add(aux);
                 
             }


                return lista;
          }
          catch (Exception ex)
          {
              throw ex;
          }
        
        
        
        
        }














      


    }

}
