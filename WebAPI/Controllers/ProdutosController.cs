using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        // injetando dependência a nossa classe de db
        private readonly APIDBContext database;

        public ProdutosController(APIDBContext context)
        {
            database = context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if (database.Produtos == null) return NotFound();

            return await database.Produtos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProduto(int id)
        {
            if (database.Produtos == null) return NotFound();

            var produto = await database.Produtos.FindAsync(id);

            if (produto == null) return NotFound();

            return Ok(produto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (database.Produtos == null) return Problem("Erro ao criar um produto");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);


            database.Produtos.Add(produto);

            // method to send data to db
            await database.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) return ValidationProblem(ModelState);
            if (!ModelState.IsValid) return ValidationProblem(ModelState);


            // safe method. sets the object state as modified
            database.Entry(produto).State = EntityState.Modified;

            // tries saving but handles if the same object is being sent twice at the same time
            try
            {
                await database.SaveChangesAsync();

            } catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id)) return NotFound();
                throw;
            }

            // método não safe --> método update não leva em conta se o objeto já está sendo modificado
            // database.Produtos.Update(produto);
            // await database.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteProduto(int id)
        {
            if (database.Produtos == null) return NotFound();

            var produto = await database.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            database.Produtos.Remove(produto);
            await database.SaveChangesAsync();

            return NoContent();
        }

        // checagem se o produto existe ou n, metodo any itera em produtos e retorna true ou false se qualquer um dos elementos na sequencia der match na expressão lambda
        // numeroId == id
        // num eh uma variavel
        private bool ProdutoExists(int id)
        {
            return (database.Produtos?.Any(num => num.Id == id)).GetValueOrDefault();
        }
    }
}
