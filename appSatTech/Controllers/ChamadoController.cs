using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using appSatTech.Models;
using Microsoft.AspNetCore.Authorization;

namespace appSatTech.Controllers
{
    [Authorize]
    public class ChamadoController : Controller
    {
        private readonly DbTecnicoContext _context;

        public ChamadoController(DbTecnicoContext context)
        {
            _context = context;
        }

        // GET: Chamado
        public async Task<IActionResult> Index(string pesquisa)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var chamados = _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .Where(c => c.ClienteId == clienteId.Value);

            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim();

                if (DateTime.TryParse(pesquisa, out var dataBusca))
                {
                    var inicio = dataBusca.Date;
                    var fim = inicio.AddDays(1);
                    chamados = chamados.Where(c => c.DataHora >= inicio && c.DataHora < fim);
                }
                else
                {
                    chamados = chamados.Where(c =>
                        c.StatusConsulta.Contains(pesquisa) ||
                        c.Tecnico.Nome.Contains(pesquisa) ||
                        c.Tecnico.Especialidade.Contains(pesquisa));
                }
            }

            ViewData["Pesquisa"] = pesquisa;

            return View(await chamados.OrderByDescending(c => c.DataHora).ToListAsync());
        }

        // GET: Chamado/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chamados == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (chamado == null)
            {
                return NotFound();
            }

            return View(chamado);
        }

        // GET: Chamado/Create
        public IActionResult Create()
        {
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "Codigo", "Nome");
            return View();
        }

        // POST: Chamado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DataHora,StatusConsulta,TecnicoId")] Chamado chamado)
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            chamado.ClienteId = clienteId.Value;
            chamado.StatusConsulta = "Agendada";

            if (ModelState.IsValid)
            {
                _context.Add(chamado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "Codigo", "Nome", chamado.TecnicoId);
            return View(chamado);
        }

        // GET: Chamado/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chamados == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Codigo", "Codigo", chamado.ClienteId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "Codigo", "Codigo", chamado.TecnicoId);
            return View(chamado);
        }

        // POST: Chamado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,DataHora,StatusConsulta,ClienteId,TecnicoId")] Chamado chamado)
        {
            if (id != chamado.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chamado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamadoExists(chamado.Codigo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Codigo", "Codigo", chamado.ClienteId);
            ViewData["TecnicoId"] = new SelectList(_context.Tecnicos, "Codigo", "Codigo", chamado.TecnicoId);
            return View(chamado);
        }

        // GET: Chamado/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Chamados == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefaultAsync(m => m.Codigo == id);
            if (chamado == null)
            {
                return NotFound();
            }

            return View(chamado);
        }

        // POST: Chamado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Chamados == null)
            {
                return Problem("Entity set 'DbTecnicoContext.Chamados'  is null.");
            }
            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado != null)
            {
                _context.Chamados.Remove(chamado);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChamadoExists(int id)
        {
          return (_context.Chamados?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
