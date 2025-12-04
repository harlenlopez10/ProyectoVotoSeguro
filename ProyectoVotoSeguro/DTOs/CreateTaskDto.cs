using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace ProyectoVotoSeguro.DTOs;

public class CreateTaskDto
{
    [Required (ErrorMessage ="El campo es requerido")]
    [MaxLength(200,ErrorMessage ="El campo titulo no puede exceder los 1000 caracteres")]
    public string Title { get; set; }
    
    [Required (ErrorMessage ="El campo es requerido")]
    [MaxLength(1000,ErrorMessage ="El campo descripcion no puede exceder los 1000 caracteres")]
    public string Description { get; set; }
    
    [Required (ErrorMessage ="El campo es requerido")]
    public string AssignedToUserId { get; set; }
    
    public DateTime?  DueDate { get; set; }
    
    [Required (ErrorMessage ="El campo es requerido")]
    [RegularExpression("(low|medium|high)$", ErrorMessage ="Prioridad invalida")]
    public string Priority { get; set; }
}