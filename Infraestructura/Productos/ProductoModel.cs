using Domain.Entities;
using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestructura.Productos
{
    public class ProductoModel
    {
        private Producto[] productos;

        #region CRUD
        public void Add(Producto p)
        {
            Add(p, ref productos);
        }
        public Producto[] GetAll()
        {
            return productos;
        }

        public int Update(Producto p)//sirve para actualizar la existencia del producto xde
        {
            if(p == null)
            {
                throw new ArgumentException("El producto no puede ser null");
            }

            int index = GetIndexById(p.Id);
            if(index < 0)
            {
                throw new Exception($"El producto con id {p.Id} no se encuenta");
            }

            productos[index] = p;
            return index;
        }

        public bool Delete(Producto p)//elimina un producto reemplazando el de la ultima pos en donde se quiere eliminar y luego se elimina el ultimo ya que este esta copiado en otra pos;
        {
            if (p == null)
            {
                throw new ArgumentException("El producto no puede ser null");
            }

            int index = GetIndexById(p.Id);
            if (index < 0)
            {
                throw new Exception($"El producto con id {p.Id} no se encuenta");
            }

            if (index != productos.Length - 1)
            {
                productos[index] = productos[productos.Length - 1];
            }

            Producto[] tmp = new Producto[productos.Length - 1];
            Array.Copy(productos, tmp, tmp.Length);
            productos = tmp;

            return productos.Length == tmp.Length;
        }
        #endregion

        #region Queries
        public Producto GetProductoById(int id)
        {
            if(id <= 0)
            {
                throw new ArgumentException($"El id: {id} no es valido.");
            }

            int index = GetIndexById(id);

            return index <= 0 ? null : productos[index];
        }
        public Producto[] GetProductosByUnidadMedida(UnidadMedida um)
        {
            Producto[] tmp = null;
            if(productos == null)
            {
                return tmp;
            }

            foreach(Producto prod in productos)
            {
                if(um == prod.UnidadMedida)
                {
                    Add(prod, ref tmp);
                }
            }
            return tmp;
        }

        public Producto[] GetProductosByVencimiento(DateTime dt)
        {
            Producto[] tmp = null;
            if (productos == null)
            {
                return tmp;
            }

            foreach (Producto prod in productos)
            {
                if (prod.FechaDeVencimiento.CompareTo(dt) <= 0)//metodo Compare to, utilizarlo en vez de "prod.FechaDeVencimiento <= dt"
                {
                    Add(prod, ref tmp);
                }
            }
                

            return tmp;
        }

        public Producto[] GetProductosByRangoPrecio(decimal start, decimal end)
        {
            Producto[] tmp = null;
            if (productos == null)
            {
                return tmp;
            }

            foreach(Producto prod in productos)
            {
                if(prod.Precio >= start && prod.Precio <= end)
                {
                    Add(prod, ref tmp);
                }
                
            }
            return tmp;
        }

        public Producto[] GetProductosOrderByPrecio()
        {
            Array.Sort(productos, new Producto.ProductoPrecioComparer());

            return productos;
        }

        public string GetProductosAsJson()
        {
            return JsonConvert.SerializeObject(productos);
        }
        #endregion

        #region Private Method
        private void Add(Producto p, ref Producto[] pds)//ref = cualquier cambio tambien se va a sufrir en el metodo que esta pasando Xde
        {
            if(pds == null)
            {
                pds = new Producto[1];
                pds[pds.Length - 1] = p;
                return;
            }

            Producto[] tmp = new Producto[pds.Length - 1];
            Array.Copy(pds, tmp, pds.Length);
            tmp[tmp.Length - 1] = p;
            pds = tmp;
        }

        private int GetIndexById(int id)//busca la posicion de un id concreto
        {
            if(id <= 0)
            {
                throw new ArgumentException("El id no puede ser 0 o negativo");
            }

            int index = int.MinValue, i = 0;

            if(productos == null)
            {
                return index;
            }

            foreach(Producto p in productos)
            {
                if(p.Id == id)
                {
                    index = i;
                    break;
                }
                i++;
            }

            return index;
        }


        #endregion

    }
}
