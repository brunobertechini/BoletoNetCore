using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BoletoNetCore.Testes.Remessa
{
    [TestFixture]
    [Category("Remessa Cnab240")]
    public class RemessaCnab240PagamentosTeste
    {
        readonly IBanco _banco;

        public RemessaCnab240PagamentosTeste()
        {
            // Conta da Empresa que receberá o valor pago do boleto
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345678",
                DigitoConta = "9",
                CarteiraPadrao = "101",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Santander);
            _banco.Beneficiario = Utils.GerarBeneficiario(
                codigoBeneficiario: "85305", 
                digitoCodigoBeneficiario: "",

                // TODO [Cnab240PagamentoFornecedor] Código do Convenio no Banco
                // Para Santander Pagamento a Fornecedor, este campo está sendo usado para
                // Código do Convenio no Banco
                // Código adotado pelo Banco para identificar o Contrato entre este e a Empresa Cliente.
                //         BBBBAAAACCCCCCCCCCCC
                //         BBBB = Número do Banco “033”
                //         AAAA = Código de Agência(sem DV)
                //         CCCCCCCCCCCC = Número do Convênio(alinhado a direita com zeros a esquerda)
                codigoTransmissao: "BBBBAAAACCCCCCCCCCCC", 

                contaBancaria: contaBancaria
            );
            _banco.FormataBeneficiario();
        }

        [Test]
        public void ArquivoRemessaCnab240PagamentoFornecedor()
        {
            var numArquivoRemessa = 1;
            var boletos = new Boletos();
            var ms = new MemoryStream();

            #region Boletos

            var boleto1 = new Boleto(_banco)
            {
                NumeroDocumento = "1000",
                DataEmissao = DateTime.Today.AddDays(-1),
                DataVencimento = DateTime.Today,
                ValorTitulo = 5,
                Carteira = "17",
                NossoNumero = "1000",
                NossoNumeroDV = String.Empty,

                Pagador = new Pagador(),
            };

            boletos.Add(boleto1);

            #endregion

            var objRemessa = new ArquivoRemessa(banco: _banco, tipoArquivo: TipoArquivo.CNAB240_Pagamento_Fornecedor, numeroArquivoRemessa: numArquivoRemessa);
            objRemessa.GerarArquivoRemessa(boletos: boletos, stream: ms, fecharRemessa: true);
        }
    }
}
