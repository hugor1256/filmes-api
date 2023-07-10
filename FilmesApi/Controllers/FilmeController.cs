using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]

public class FilmeController : ControllerBase
{

    private readonly FilmeContext _contextFilmes;
    private readonly IMapper _mapper;

    public FilmeController(FilmeContext contextFilmes, IMapper mapper)
    {
        _contextFilmes = contextFilmes;
        _mapper = mapper;
    }
    
    private static List<Filme> filmes = new List<Filme>();
    private static int id = 0;


    [HttpPost]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _contextFilmes.Filmes.Add(filme);
        _contextFilmes.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filmes);

    }
 
    [HttpGet]
    public IEnumerable<Filme> RecuperaFilmes([FromQuery]int skip = 0, [FromQuery]int take = 50)
    {
        return _contextFilmes.Filmes.Skip(skip).Take(take);
    }
    
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _contextFilmes.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        return Ok(filme);
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _contextFilmes.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _contextFilmes.SaveChanges();
        return NoContent();

    }
    
    [HttpPatch("{id}")]
    public IActionResult AtualizarFilmeParcial(int id, [FromBody] JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _contextFilmes.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
        
        patch.ApplyTo(filmeParaAtualizar, ModelState);
        
        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        
        _mapper.Map(filmeParaAtualizar, filme);
        _contextFilmes.SaveChanges();
        return NoContent();

    }
}