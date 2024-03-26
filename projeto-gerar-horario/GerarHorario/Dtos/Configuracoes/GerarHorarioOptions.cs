using Sisgea.GerarHorario.Core.Dtos.Entidades;

namespace Sisgea.GerarHorario.Core.Dtos.Configuracoes;

public class GerarHorarioOptions
{

    public int DiaSemanaInicio { get; set; }
    public int DiaSemanaFim { get; set; }

    public Turma[] Turmas { get; set; }
    public Professor[] Professores { get; set; }
    public Intervalo[] HorariosDeAula { get; set; }

    public override string ToString()
    {
        return "GerarHorarioOptions { nenhuma configuração }";
    }
}