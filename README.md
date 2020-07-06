# Oragon.Common.RingBuffer
Implementação de Ring/Circular Buffer com .NET Standard para .NET



# Roadmap

## Criação
* :heavy_check_mark: Criação do projeto e primeira versão. 
* :heavy_check_mark: Sem licença e sem permissão de uso.

## Lapidação
* :fire: Identificação de oportunidades e gaps
* :radio_button: Revisão da modelagem
* :radio_button: Análise de alocação de memória
* :radio_button: Definição de roadmap

## Automação
* :radio_button: Build - Pipeline do Jenkins
* :radio_button: Pack - Pipeline do Jenkins
* :radio_button: Deploy - Pipeline do Jenkins - MyGet
* :radio_button: Deploy - Pipeline do Jenkins - NuGet
* :radio_button: Notification - Pipeline do Jenkins + Twitter

## Abertura
* :radio_button: Definir e aplicar licenças
  * Permitir uso Irrestrito
  * Não permitir modificação


# Decision Log

## Objetivo

* Reduzir complexidade na implementação de buffers de conexão genéricos.
* Otimizar o consumo de recursos em aplicações que precisam se conectar a outros serviços cujo custo de conexão e handshake é elevado.

## Premissas e Ambições
* Implementar um ring buffer
* Permitir a extensibilidade dos controllers para que possa ser aplicado um ciclo de vida
* Permitir revitalização / recriação do item do buffer
* Adicionar instrumentação para permitir a criação de métricas
* Permitir que, com o uso das métricas, seja possível criar um buffer elástico

# Posts

[![Ring Buffer – Antecipe, otimize e evite custos excessivos][ring-buffer2]](https://gago.io/blog/ring-buffer-quase-um-uber/?utm_source=github&utm_medium=luizcarlosfaria&utm_content=Oragon.Common.RingBuffer) 
Ring Buffer, também chamado de Circular Buffer é uma estrutura de dados muito poderosa. Seu nome já traz o spoiler e entrega o ouro, afinal não deixa de ser um buffer, só que trabalhando em formato de anel/circular. Se você não faz ideia do que seja, vem comigo nessa viagem pois vamos dissecar o assunto e ainda compará-la ao processo de uberização dos objetos custosos.


:copyright: [Luiz Carlos Faria](http://gago.io/) ![made in brazil][brazil]

[brazil]:http://www.goal.cc/content/images/flags/28.png
[ring-buffer2]:https://gago.io/wp-content/uploads/2020/07/ring-buffer2.png

