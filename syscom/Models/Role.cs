using System;
using System.Collections.Generic;

namespace syscom.Models;

public partial class Role
{
    public int Id { get; set; }

    public string NombreCargo { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
