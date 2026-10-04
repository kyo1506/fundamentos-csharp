namespace Fase05.GestaoAcademica.Core;

/// <summary>
/// Contrato do padrão Strategy para cálculo de descontos em matrículas.
/// </summary>
public interface IPoliticaDesconto
{
    decimal CalcularDesconto(decimal precoBase, Aluno aluno);
}

/// <summary>
/// Política de desconto para alunos com convênio empresarial (25% de desconto).
/// </summary>
public class DescontoConvenioPolicy : IPoliticaDesconto
{
    public decimal CalcularDesconto(decimal precoBase, Aluno aluno)
    {
        return aluno.PossuiConvenioEmpresarial ? precoBase * 0.25m : 0m;
    }
}

/// <summary>
/// Política de desconto promocional padrão (10% de desconto geral).
/// </summary>
public class DescontoPromocionalPolicy : IPoliticaDesconto
{
    public decimal CalcularDesconto(decimal precoBase, Aluno aluno)
    {
        return precoBase * 0.10m;
    }
}

/// <summary>
/// Política neutra sem nenhum desconto.
/// </summary>
public class SemDescontoPolicy : IPoliticaDesconto
{
    public decimal CalcularDesconto(decimal precoBase, Aluno aluno) => 0m;
}
