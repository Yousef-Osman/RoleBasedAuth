using System.ComponentModel.DataAnnotations;

namespace RoleBasedAuth.ViewModels;

public class RoleVM
{
    public string Id { get; set; }

    [Required, Display(Name = "Role Name")]
    public string Name { get; set; }

    public bool IsSelected { get; set; }
}
