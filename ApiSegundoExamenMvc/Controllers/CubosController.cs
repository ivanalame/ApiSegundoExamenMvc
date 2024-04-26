using ApiSegundoExamenMvc.Models;
using ApiSegundoExamenMvc.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiSegundoExamenMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CubosController : ControllerBase
    {
        private RepositoryCubos repo;

        public CubosController(RepositoryCubos repo)
        {
            this.repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cubo>>> GetCubos()
        {
            return await this.repo.GetCubosAsync();
        }
        [HttpGet("{marca}")]
        public async Task <ActionResult<List<Cubo>>>GetCubosByMarca(string marca)
        {
            return await this.repo.FindCubosByMarca(marca);
        }
        [HttpPost]
        public async Task<ActionResult>PostUsuario(Usuario usuario)
        {
            await this.repo.InsertUser(usuario.Nombre, usuario.Email, usuario.Password, usuario.Imagen);
            return Ok();
        }

        //METODO CON AUTHORIZE
        //PERFIL
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>>PerfilUsuario()
        {
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UserData");
            string jsonUsuario = claim.Value;
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            string ruta = await this.repo.GetContainerPathAsync();
            usuario.Imagen = ruta + "/" + usuario.Imagen;
            return usuario;
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]

        public async Task<ActionResult<List<CompraCubo>>> PedidoUsuario()
        {
            Claim claim = HttpContext.User.FindFirst(x => x.Type == "UserData");
            string jsonUsuario = claim.Value;

            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return await this.repo.PedidosUsuario(usuario);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult>PostPedido(CompraCubo compra)
        {
            await this.repo.InsertPedido(compra.IdCubo, compra.IdUsuario);
            return Ok();
        }
    }
}
