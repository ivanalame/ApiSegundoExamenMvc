using ApiSegundoExamenMvc.Data;
using ApiSegundoExamenMvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ApiSegundoExamenMvc.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            return await this.context.Cubos.ToListAsync();
        }
        public async Task<List<Cubo>>FindCubosByMarca(string marca)
        {
            return await this.context.Cubos.Where(x => x.Marca == marca).ToListAsync();
        }
        public async Task InsertUser(string nombre,string email,string pass,string imagen)
        {
            int maxid = await this.context.Usuarios.MaxAsync(z => z.IdUsuario) + 1;
            Usuario user = new Usuario();
            user.IdUsuario = maxid;
            user.Nombre = nombre;
            user.Email = email;
            user.Password = pass;
            user.Imagen = imagen;

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();
        }

        //METODOS CON AUTHORIZE
       
        public async Task<Usuario>PerfilUsuarioAsync(int IdUsuario)
        {
            return await this.context.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == IdUsuario);
        }

        public async Task<List<CompraCubo>> PedidosUsuario(Usuario usuario)
        {
            return await this.context.ComprasCubo
                .Where(x => x.IdUsuario == usuario.IdUsuario)
                .ToListAsync();
        }
        //insert compra
        public async Task InsertPedido(int idcubo, int idusuario)
        {
            int maxid = await this.context.ComprasCubo.MaxAsync(z => z.IdPedido) + 1;
             CompraCubo pedido = new CompraCubo();
            pedido.IdPedido = maxid;
            pedido.IdCubo = idcubo;
            pedido.IdUsuario = idusuario;
            pedido.FechaPedido= DateTime.Now;

            this.context.ComprasCubo.Add(pedido);
            await this.context.SaveChangesAsync();
        }

        //log in user
        public async Task<Usuario>LogInUsuarioAsync(string email,string password)
        {
            return await this.context.Usuarios.Where(x=>x.Email == email && x.Password == password).FirstOrDefaultAsync();
        }
    }
}
