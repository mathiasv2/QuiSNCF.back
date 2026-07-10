using Microsoft.AspNetCore.Mvc;
using QuiSNCF.Repository;

namespace QuiSNCF.API.Controllers;

[Route("api/[controller]")]
public class CityController(ICityRepository repo): ControllerBase
{
    
    
}