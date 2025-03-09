using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using syscom.Models;
using syscom.Services;

namespace syscom.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly SyscomdbContext _context;
        private readonly DiasFestivosService _diasFestivosService;

        public UsuariosController(SyscomdbContext context, DiasFestivosService diasFestivosService)
        {
            _context = context;
            _diasFestivosService = diasFestivosService;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var syscomdbContext = _context.Usuarios.Include(u => u.IdRolNavigation);
            var usuarios = await syscomdbContext.ToListAsync();
            List<DateTime> diasFestivos = await _diasFestivosService.ObtenerDiasFestivosAsync();
            
            foreach (var usuario in usuarios)
            {
                usuario.DiasHabilesTrabajados = CalcularDiasHabiles(usuario.FechaIngreso, usuario.FechaEliminacion, diasFestivos);
            }
            return View(await syscomdbContext.ToListAsync());
        }
        
        private int CalcularDiasHabiles(DateOnly inicio, DateOnly? fin, List<DateTime> diasFestivos)
        {
            DateTime fechaInicio = inicio.ToDateTime(TimeOnly.MinValue);
            DateTime fechaFin = fin?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today;

            int diasHabiles = 0;

            while (fechaInicio <= fechaFin)
            {
                bool esFinDeSemana = fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday;
                bool esFestivo = diasFestivos.Contains(fechaInicio);

                if (!esFinDeSemana && !esFestivo)
                {
                    diasHabiles++;
                }
                fechaInicio = fechaInicio.AddDays(1);
            }

            return diasHabiles;
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "NombreCargo");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,CorreoElectronico,IdRol,FechaIngreso,Firma,Contrato,FechaEliminacion")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                string rutaPdf = GenerarPdf(usuario);

                // Guardar la ruta en la base de datos
                usuario.Contrato = rutaPdf;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        //Generador de PDF del contrato
        private string GenerarPdf(Usuario usuario)
        {
            // Ruta donde se guardarán los PDFs (puedes cambiarla según sea necesario)
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Contratos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath); // Crear la carpeta si no existe
            }

            // Nombre del archivo con el ID del usuario
            string fileName = $"Contrato_{usuario.Nombre}.pdf";
            string filePath = Path.Combine(folderPath, fileName);

            // Crear el PDF usando iText7
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                PdfWriter writer = new PdfWriter(stream, new WriterProperties());
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                document.Add(new Paragraph("Bogotá,Colombia").SetFontSize(11));
                document.Add(new Paragraph($"Fecha de Creación: {usuario.FechaIngreso}"));
                document.Add(new Paragraph($"Por medio del presente contrato se ratifica que {usuario.Nombre} Acepta los terminos y condiciones"));
                document.Add(new Paragraph($"Firma: {usuario.Firma}"));


                document.Close();
            }

            // Retornar la ruta relativa del PDF para guardarla en la base de datos
            return $"/Contratos/{fileName}";
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,CorreoElectronico,IdRol,FechaIngreso,Firma,Contrato,FechaEliminacion")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            ViewData["IdRol"] = new SelectList(_context.Roles, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
