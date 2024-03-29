using Sisgea.GerarHorario.Core.Dtos.Entidades;

namespace Sisgea.GerarHorario.Core.Dtos.Configuracoes;

public class GerarHorarioOptions
{

    public int DiaSemanaInicio { get; set; }
    public int DiaSemanaFim { get; set; }

    public Turma[] Turmas { get; set; }
    public Professor[] Professores { get; set; }
    public Intervalo[] HorariosDeAula { get; set; }

    public bool LogDebug { get; set; }

    public GerarHorarioOptions(int diaSemanaInicio, int diaSemanaFim, Turma[] turmas, Professor[] professores, Intervalo[] horariosDeAula, bool logDebug = false)
    {
        DiaSemanaInicio = diaSemanaInicio;
        DiaSemanaFim = diaSemanaFim;
        Turmas = turmas;
        Professores = professores;
        HorariosDeAula = horariosDeAula;
        LogDebug = logDebug;
    }


    public override string ToString()
    {
        return "GerarHorarioOptions { nenhuma configuração }";
    }
}
