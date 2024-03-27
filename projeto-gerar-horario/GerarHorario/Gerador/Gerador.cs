using Google.OrTools.Sat;
using Sisgea.GerarHorario.Core.Dtos.Configuracoes;
using Sisgea.GerarHorario.Core.Dtos.HorarioGerado;

namespace Sisgea.GerarHorario.Core;

public class Gerador
{
    public static GerarHorarioContext PrepararModelComRestricoes(GerarHorarioOptions options, bool debug = false)
    {
        // ================================================
        var contexto = new GerarHorarioContext(options);
        // model -> contexto.Model;
        // options -> contexto.Options;
        // todasAsPropostasDeAula -> contexto.TodasAsPropostasDeAula;
        // ================================================

        for (int diaSemanaIso = options.DiaSemanaInicio; diaSemanaIso <= options.DiaSemanaFim; diaSemanaIso++)
        {
            for (int intervaloIndex = 0; intervaloIndex < options.HorariosDeAula.Length; intervaloIndex++)
            {
                foreach (var turma in options.Turmas)
                {
                    foreach (var diario in turma.DiariosDaTurma)
                    {
                        var propostaLabel = $"dia_{diaSemanaIso}::intervalo_{intervaloIndex}::diario_{diario.Id}";

                        var modelBoolVar = contexto.Model.NewBoolVar(propostaLabel);

                        var propostaDeAula = new PropostaAula(turma.Id, diario.Id, diaSemanaIso, intervaloIndex, modelBoolVar);

                        contexto.TodasAsPropostasDeAula.Add(propostaDeAula);

                        if (debug)
                        {
                            Console.WriteLine($"--> init proposta de aula | {propostaLabel}");
                        }
                    }
                }
            }
        }

        // ======================================

        // RESTRIÇÃO: Garantir no máximo 1 aula em um (dia e intervalo) para cada turma.

        foreach (var turma in options.Turmas)
        {
            foreach (var diaSemanaIso in Enumerable.Range(options.DiaSemanaInicio, options.DiaSemanaFim))
            {
                foreach (var intervaloIndex in Enumerable.Range(0, options.HorariosDeAula.Length))
                {
                    var propostas = from propostaAula in contexto.TodasAsPropostasDeAula
                                    where
                                       propostaAula.DiaSemanaIso == diaSemanaIso // mesmo dia
                                       && propostaAula.IntervaloIndex == intervaloIndex // mesmo horário
                                       && turma.DiariosDaTurma.Any(diario => diario.Id == propostaAula.DiarioId)
                                    select propostaAula.ModelBoolVar;


                    var propostasList = propostas.ToList();

                    Console.WriteLine($"Turma: {turma.Id} | Dia: {diaSemanaIso} | Intervalo: {intervaloIndex} | Propostas: {propostasList.Count}");

                    contexto.Model.AddAtMostOne(propostasList);
                }
            }
        }

        // ==========================================================================================================

        // RESTRIÇÃO: Diário: quantidade máxima na semana
        Restricoes.AplicarLimiteDiarioSemana(contexto);

        // ==========================================================================================================

        // TODO: todas as restrições são implementadas aqui.

        // ...

        // ==========================================================================================================

        LinearExprBuilder score = LinearExpr.NewBuilder();

        foreach (var propostaDeAula in contexto.TodasAsPropostasDeAula)
        {
            score.AddTerm((IntVar)propostaDeAula.ModelBoolVar, 1);
        }

        contexto.Model.Maximize(score);

        // ==========================================================================================================

        return contexto;
    }

    public static IEnumerable<HorarioGerado> GerarHorario(
      GerarHorarioOptions options,
      bool verbose = false)
    {
        // CRIA UM MODELO COM AS RESTRIÇÕES VINDAS DAS OPÇÕES
        var contexto = PrepararModelComRestricoes(options, verbose);

        // RESOLVE O MODELO
        var solver = new CpSolver();
        var status = solver.Solve(contexto.Model);

        // STATUS DA SOLUÇÃO
        Console.WriteLine($"Solve status: {status}");

        // Mostra a solução.
        // Check that the problem has a feasible solution.
        if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
        {
            Console.WriteLine($"Total cost: {solver.ObjectiveValue}\n");
        }
        else
        {
            Console.WriteLine("No solution found.");
        }

        Console.WriteLine("============================");
        Console.WriteLine("Statistics");
        Console.WriteLine($"  - conflicts : {solver.NumConflicts()}");
        Console.WriteLine($"  - branches  : {solver.NumBranches()}");
        Console.WriteLine($"  - wall time : {solver.WallTime()}s");
        Console.WriteLine("============================");
        Console.WriteLine("");

        // ====================================================================================

        // TODO: gerar mais de um horário com CpSolverSolutionCallback OnSolutionCallback

        // ....

        // ==================

        // Filtrar propostasDeAula com modelBoolVar == true na solução atual

        var propostasAtivas = from propostaAula in contexto.TodasAsPropostasDeAula
                              where
                                solver.BooleanValue(propostaAula.ModelBoolVar)
                              select new HorarioGeradoAula(propostaAula.TurmaId, propostaAula.DiarioId, propostaAula.IntervaloIndex, propostaAula.DiaSemanaIso);

        var horarioGerado = new HorarioGerado
        {
            Aulas = propostasAtivas.ToArray()
        };

        yield return horarioGerado;

        // ====================================================================================

    }
}

