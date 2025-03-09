using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace syscom.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    [Display(Name = "Correo Electronico")]
    public string CorreoElectronico { get; set; } = null!;

    [Display(Name = "Cargo")]
    public int? IdRol { get; set; }

    [Display(Name = "Fecha Ingreso")]
    public DateOnly FechaIngreso { get; set; }

    public string? Firma { get; set; }

    public string? Contrato { get; set; }

    public DateOnly? FechaEliminacion { get; set; }

    [Display(Name = "Cargo")]
    public virtual Role? IdRolNavigation { get; set; }

    [NotMapped]
    [Display(Name = "Dias trabajados")]
    public int DiasHabilesTrabajados { get; set; }
}
