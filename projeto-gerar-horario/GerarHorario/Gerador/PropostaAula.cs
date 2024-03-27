using Google.OrTools.Sat;

namespace Sisgea.GerarHorario.Core;

public class PropostaAula(
    int diaSemanaIso,
    int intervaloIndex,
    string diarioId,
    BoolVar modelBoolVar
    )
{
    public int diaSemanaIso = diaSemanaIso;
    public int intervaloIndex = intervaloIndex;
    public string diarioId = diarioId;
    public BoolVar modelBoolVar = modelBoolVar;
}