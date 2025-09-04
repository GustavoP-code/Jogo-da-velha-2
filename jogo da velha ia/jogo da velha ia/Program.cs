using System;

class JogoDaVelha
{
    static char[] tabuleiro;
    static int jogadorAtual;
    static int escolha;
    static int vencedor;
    static bool modoVsComputador;
    static int dificuldadeIA; // 1 = Fácil, 2 = Difícil
    static Random random = new Random();
    static bool primeiraJogadaHumano = true;
    static string ultimaMensagemComputador = "";

    // Sistema de pontuação completo
    static int pontuacaoJogador1 = 0;
    static int pontuacaoJogador2 = 0;
    static int pontuacaoComputador = 0;
    static int empates = 0;
    static int modoJogoAtual = 0; // 0 = nenhum, 1 = PvP, 2 = PvCPU

    static void Main(string[] args)
    {
        bool sairDoJogo = false;

        while (!sairDoJogo)
        {
            Console.Clear();
            Console.WriteLine("Integrantes:");
            Console.WriteLine("Arthur Orosco Campos");
            Console.WriteLine("Gustavo Pereira da Silva");
            Console.WriteLine("Jean Carlos Belino\n");
            MostrarPlacarCompleto();
            Console.WriteLine("\n=== Jogo da Velha ===");
            Console.WriteLine("\nMenu Principal:");
            Console.WriteLine("1 - Jogador vs Jogador");
            Console.WriteLine("2 - Jogador vs Computador");
            Console.WriteLine("3 - Sair");
            Console.Write("\nEscolha uma opção: ");

            int opcaoMenu;
            while (!int.TryParse(Console.ReadLine(), out opcaoMenu) || (opcaoMenu < 1 || opcaoMenu > 3))
            {
                Console.Write("Opção inválida. Digite 1, 2 ou 3: ");
            }

            if (opcaoMenu == 3)
            {
                sairDoJogo = true;
                continue;
            }

            // Reinicia o placar se o modo de jogo mudou
            if (modoJogoAtual != opcaoMenu)
            {
                ZerarPlacar();
                modoJogoAtual = opcaoMenu;
            }

            modoVsComputador = (opcaoMenu == 2);

            if (modoVsComputador)
            {
                Console.Clear();
                MostrarPlacarCompleto();
                Console.WriteLine("\n=== Dificuldade da IA ===");
                Console.WriteLine("1 - Fácil (jogadas aleatórias)");
                Console.WriteLine("2 - Difícil (estratégia avançada)");
                Console.Write("\nEscolha a dificuldade: ");

                while (!int.TryParse(Console.ReadLine(), out dificuldadeIA) || (dificuldadeIA < 1 || dificuldadeIA > 2))
                {
                    Console.Write("Opção inválida. Digite 1 ou 2: ");
                }
            }

            bool jogarNovamente = true;

            while (jogarNovamente)
            {
                InicializarJogo();

                Console.Clear();
                MostrarPlacarCompleto();
                Console.WriteLine(modoVsComputador ?
                    $"\nModo: Jogador vs Computador (Dificuldade: {(dificuldadeIA == 1 ? "Fácil" : "Difícil")}" :
                    "\nModo: Jogador vs Jogador");

                // Decidir aleatoriamente quem começa no modo vs Computador
                if (modoVsComputador)
                {
                    jogadorAtual = random.Next(1, 3); // 1 ou 2
                    Console.WriteLine($"\n{(jogadorAtual == 1 ? "Você começa jogando!" : "O computador começa jogando!")}");
                }

                do
                {
                    Console.Clear();
                    MostrarPlacarCompleto();
                    Console.WriteLine(modoVsComputador ?
                        "\nJogador: X | Computador: O" :
                        "\nJogador 1: X | Jogador 2: O");
                    Console.WriteLine();

                    DesenharTabuleiro();

                    // Mostra a última jogada do computador abaixo do tabuleiro
                    if (!string.IsNullOrEmpty(ultimaMensagemComputador))
                    {
                        Console.WriteLine(ultimaMensagemComputador);
                        ultimaMensagemComputador = ""; // Limpa a mensagem após mostrar
                    }

                    // Verifica empate ANTES de qualquer jogada
                    if (vencedor == 0 && !ExistemJogadasDisponiveis())
                    {
                        // EMPATE - Atualiza imediatamente
                        empates++;
                        Console.Clear();
                        MostrarPlacarCompleto();
                        Console.WriteLine(modoVsComputador ?
                            "\nJogador: X | Computador: O" :
                            "\nJogador 1: X | Jogador 2: O");
                        Console.WriteLine();
                        DesenharTabuleiro();
                        Console.WriteLine("\nEmpate!");
                        break;
                    }

                    if (vencedor != 0)
                    {
                        // Já foi tratado anteriormente com break
                        break;
                    }

                    if (jogadorAtual == 1 || !modoVsComputador)
                    {
                        // Vez do jogador humano
                        string mensagem = primeiraJogadaHumano ? "Sua vez" : "Sua vez de jogar";
                        Console.WriteLine($"\n{mensagem}. Escolha uma posição (1-9): ");
                        primeiraJogadaHumano = false;

                        bool conversaoValida = int.TryParse(Console.ReadLine(), out escolha);

                        if (conversaoValida && escolha >= 1 && escolha <= 9 && tabuleiro[escolha - 1] != 'X' && tabuleiro[escolha - 1] != 'O')
                        {
                            char marca = (jogadorAtual == 1) ? 'X' : 'O';
                            tabuleiro[escolha - 1] = marca;

                            if (VerificarVencedor(marca))
                            {
                                vencedor = jogadorAtual;

                                // Atualiza pontuação IMEDIATAMENTE
                                if (modoVsComputador)
                                {
                                    if (vencedor == 1) pontuacaoJogador1++;
                                    else pontuacaoComputador++;
                                }
                                else
                                {
                                    if (vencedor == 1) pontuacaoJogador1++;
                                    else pontuacaoJogador2++;
                                }

                                // Mostra tela final com placar ATUALIZADO imediatamente
                                Console.Clear();
                                MostrarPlacarCompleto();
                                Console.WriteLine(modoVsComputador ?
                                    "\nJogador: X | Computador: O" :
                                    "\nJogador 1: X | Jogador 2: O");
                                Console.WriteLine();
                                DesenharTabuleiro();

                                if (modoVsComputador)
                                {
                                    Console.WriteLine(vencedor == 1 ?
                                        "\nParabéns! Você venceu!" :
                                        "\nO computador venceu!");
                                }
                                else
                                {
                                    Console.WriteLine($"\nJogador {vencedor} venceu!");
                                }

                                break; // Sai do loop imediatamente após vitória
                            }
                            else if (ExistemJogadasDisponiveis())
                            {
                                if (modoVsComputador)
                                {
                                    jogadorAtual = 3 - jogadorAtual; // Alterna entre 1 e 2

                                    // Se for a vez do computador, ele joga imediatamente
                                    if (jogadorAtual == 2 && vencedor == 0 && ExistemJogadasDisponiveis())
                                    {
                                        int posicaoComputador = JogadaComputador();
                                        ultimaMensagemComputador = $"\nO computador jogou na posição {posicaoComputador + 1}";

                                        if (VerificarVencedor('O'))
                                        {
                                            vencedor = 2;
                                            pontuacaoComputador++;
                                            Console.Clear();
                                            MostrarPlacarCompleto();
                                            Console.WriteLine("\nJogador: X | Computador: O");
                                            Console.WriteLine();
                                            DesenharTabuleiro();
                                            Console.WriteLine("\nO computador venceu!");
                                            break; // Sai do loop imediatamente
                                        }

                                        // Verifica empate APÓS jogada do computador
                                        if (!ExistemJogadasDisponiveis())
                                        {
                                            empates++;
                                            Console.Clear();
                                            MostrarPlacarCompleto();
                                            Console.WriteLine("\nJogador: X | Computador: O");
                                            Console.WriteLine();
                                            DesenharTabuleiro();
                                            Console.WriteLine("\nEmpate!");
                                            break;
                                        }

                                        if (vencedor == 0 && ExistemJogadasDisponiveis())
                                        {
                                            jogadorAtual = 1; // Volta para o jogador humano
                                        }
                                    }
                                }
                                else
                                {
                                    jogadorAtual = (jogadorAtual == 1) ? 2 : 1;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Jogada inválida. Tente novamente.");
                        }
                    }
                    else
                    {
                        // Vez do computador
                        int posicaoComputador = JogadaComputador();
                        ultimaMensagemComputador = $"\nO computador jogou na posição {posicaoComputador + 1}";

                        if (VerificarVencedor('O'))
                        {
                            vencedor = 2;
                            pontuacaoComputador++;
                            Console.Clear();
                            MostrarPlacarCompleto();
                            Console.WriteLine("\nJogador: X | Computador: O");
                            Console.WriteLine();
                            DesenharTabuleiro();
                            Console.WriteLine("\nO computador venceu!");
                            break; // Sai do loop imediatamente
                        }

                        // Verifica empate APÓS jogada do computador
                        if (!ExistemJogadasDisponiveis())
                        {
                            empates++;
                            Console.Clear();
                            MostrarPlacarCompleto();
                            Console.WriteLine("\nJogador: X | Computador: O");
                            Console.WriteLine();
                            DesenharTabuleiro();
                            Console.WriteLine("\nEmpate!");
                            break;
                        }

                        if (vencedor == 0 && ExistemJogadasDisponiveis())
                        {
                            jogadorAtual = 1; // Volta para o jogador humano
                            primeiraJogadaHumano = false; // Não é mais a primeira jogada
                        }
                    }

                } while (vencedor == 0 && ExistemJogadasDisponiveis());

                // Se houve empate e não foi detectado dentro do loop, mostra a tela final
                if (vencedor == 0 && !ExistemJogadasDisponiveis())
                {
                    empates++;
                    Console.Clear();
                    MostrarPlacarCompleto();
                    Console.WriteLine(modoVsComputador ?
                        "\nJogador: X | Computador: O" :
                        "\nJogador 1: X | Jogador 2: O");
                    Console.WriteLine();
                    DesenharTabuleiro();
                    Console.WriteLine("\nEmpate!");
                }

                Console.WriteLine("\nO que deseja fazer agora?");
                Console.WriteLine("1 - Jogar novamente no mesmo modo");
                Console.WriteLine("2 - Voltar ao menu principal");
                Console.WriteLine("3 - Sair do jogo");
                Console.WriteLine("4 - Zerar placar");
                Console.Write("\nEscolha uma opção: ");

                int opcaoPosJogo;
                while (!int.TryParse(Console.ReadLine(), out opcaoPosJogo) || (opcaoPosJogo < 1 || opcaoPosJogo > 4))
                {
                    Console.Write("Opção inválida. Digite 1, 2, 3 ou 4: ");
                }

                switch (opcaoPosJogo)
                {
                    case 1:
                        jogarNovamente = true;
                        break;
                    case 2:
                        jogarNovamente = false;
                        break;
                    case 3:
                        jogarNovamente = false;
                        sairDoJogo = true;
                        break;
                    case 4:
                        ZerarPlacar();
                        jogarNovamente = false;
                        break;
                }
            }
        }

        Console.WriteLine("\nObrigado por jogar! Até a próxima!");
    }

    private static void MostrarPlacarCompleto()
    {
        Console.WriteLine("=== PLACAR ===");
        Console.WriteLine($"Jogador 1: {pontuacaoJogador1} vitória(s)");
        Console.WriteLine($"Jogador 2: {pontuacaoJogador2} vitória(s)");
        Console.WriteLine($"Computador: {pontuacaoComputador} vitória(s)");
        Console.WriteLine($"Empates: {empates}");
        Console.WriteLine("==============");
    }

    private static void ZerarPlacar()
    {
        pontuacaoJogador1 = 0;
        pontuacaoJogador2 = 0;
        pontuacaoComputador = 0;
        empates = 0;
    }

    private static void InicializarJogo()
    {
        tabuleiro = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        vencedor = 0;
        primeiraJogadaHumano = true;
        ultimaMensagemComputador = "";
        if (!modoVsComputador)
        {
            jogadorAtual = 1;
        }
    }

    private static int JogadaComputador()
    {
        int jogada = -1;

        if (dificuldadeIA == 1)
        {
            jogada = JogadaFacil();
        }
        else
        {
            jogada = JogadaDificil();
        }

        return jogada;
    }

    private static int JogadaFacil()
    {
        int posicao;
        do
        {
            posicao = random.Next(0, 9);
        } while (tabuleiro[posicao] == 'X' || tabuleiro[posicao] == 'O');

        tabuleiro[posicao] = 'O';
        return posicao;
    }

    private static int JogadaDificil()
    {
        // Usa o algoritmo Minimax para encontrar a melhor jogada
        int melhorPontuacao = int.MinValue;
        int melhorJogada = -1;

        // Para cada jogada possível
        for (int i = 0; i < 9; i++)
        {
            if (tabuleiro[i] != 'X' && tabuleiro[i] != 'O')
            {
                // Faz a jogada
                char originalValor = tabuleiro[i];
                tabuleiro[i] = 'O';

                // Calcula a pontuação usando Minimax
                int pontuacao = Minimax(tabuleiro, 0, false);

                // Desfaz a jogada
                tabuleiro[i] = originalValor;

                // Atualiza a melhor jogada
                if (pontuacao > melhorPontuacao)
                {
                    melhorPontuacao = pontuacao;
                    melhorJogada = i;
                }
            }
        }

        // Faz a melhor jogada
        tabuleiro[melhorJogada] = 'O';
        return melhorJogada;
    }

    private static int Minimax(char[] tabuleiroAtual, int profundidade, bool ehMaximizador)
    {
        // Verifica se o jogo terminou
        if (VerificarVencedor('O', tabuleiroAtual))
            return 10 - profundidade;

        if (VerificarVencedor('X', tabuleiroAtual))
            return profundidade - 10;

        if (!ExistemJogadasDisponiveis(tabuleiroAtual))
            return 0;

        if (ehMaximizador) // Vez da IA (O)
        {
            int melhorPontuacao = int.MinValue;

            for (int i = 0; i < 9; i++)
            {
                if (tabuleiroAtual[i] != 'X' && tabuleiroAtual[i] != 'O')
                {
                    char original = tabuleiroAtual[i];
                    tabuleiroAtual[i] = 'O';
                    int pontuacao = Minimax(tabuleiroAtual, profundidade + 1, false);
                    tabuleiroAtual[i] = original;

                    melhorPontuacao = Math.Max(melhorPontuacao, pontuacao);
                }
            }

            return melhorPontuacao;
        }
        else // Vez do jogador (X)
        {
            int piorPontuacao = int.MaxValue;

            for (int i = 0; i < 9; i++)
            {
                if (tabuleiroAtual[i] != 'X' && tabuleiroAtual[i] != 'O')
                {
                    char original = tabuleiroAtual[i];
                    tabuleiroAtual[i] = 'X';
                    int pontuacao = Minimax(tabuleiroAtual, profundidade + 1, true);
                    tabuleiroAtual[i] = original;

                    piorPontuacao = Math.Min(piorPontuacao, pontuacao);
                }
            }

            return piorPontuacao;
        }
    }

    private static int EncontrarJogadaVencedora(char marca)
    {
        for (int i = 0; i < 9; i++)
        {
            if (tabuleiro[i] != 'X' && tabuleiro[i] != 'O')
            {
                char original = tabuleiro[i];
                tabuleiro[i] = marca;

                if (VerificarVencedor(marca))
                {
                    tabuleiro[i] = original;
                    return i;
                }

                tabuleiro[i] = original;
            }
        }
        return -1;
    }

    private static bool VerificarVencedor(char marca)
    {
        return VerificarVencedor(marca, tabuleiro);
    }

    private static bool VerificarVencedor(char marca, char[] tabuleiroAtual)
    {
        if ((tabuleiroAtual[0] == marca && tabuleiroAtual[1] == marca && tabuleiroAtual[2] == marca) ||
            (tabuleiroAtual[3] == marca && tabuleiroAtual[4] == marca && tabuleiroAtual[5] == marca) ||
            (tabuleiroAtual[6] == marca && tabuleiroAtual[7] == marca && tabuleiroAtual[8] == marca) ||
            (tabuleiroAtual[0] == marca && tabuleiroAtual[3] == marca && tabuleiroAtual[6] == marca) ||
            (tabuleiroAtual[1] == marca && tabuleiroAtual[4] == marca && tabuleiroAtual[7] == marca) ||
            (tabuleiroAtual[2] == marca && tabuleiroAtual[5] == marca && tabuleiroAtual[8] == marca) ||
            (tabuleiroAtual[0] == marca && tabuleiroAtual[4] == marca && tabuleiroAtual[8] == marca) ||
            (tabuleiroAtual[2] == marca && tabuleiroAtual[4] == marca && tabuleiroAtual[6] == marca))
        {
            return true;
        }
        return false;
    }

    private static bool ExistemJogadasDisponiveis()
    {
        return ExistemJogadasDisponiveis(tabuleiro);
    }

    private static bool ExistemJogadasDisponiveis(char[] tabuleiroAtual)
    {
        foreach (char posicao in tabuleiroAtual)
        {
            if (posicao != 'X' && posicao != 'O')
            {
                return true;
            }
        }
        return false;
    }

    private static void DesenharTabuleiro()
    {
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {ObterCaractereColorido(tabuleiro[0])}  |  {ObterCaractereColorido(tabuleiro[1])}  |  {ObterCaractereColorido(tabuleiro[2])}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {ObterCaractereColorido(tabuleiro[3])}  |  {ObterCaractereColorido(tabuleiro[4])}  |  {ObterCaractereColorido(tabuleiro[5])}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {ObterCaractereColorido(tabuleiro[6])}  |  {ObterCaractereColorido(tabuleiro[7])}  |  {ObterCaractereColorido(tabuleiro[8])}  ");
        Console.WriteLine("     |     |     ");
    }

    private static string ObterCaractereColorido(char caractere)
    {
        if (caractere == 'X')
        {
            // Amarelo
            return "\u001b[94mX\u001b[0m";
        }
        else if (caractere == 'O')
        {
            // Vermelho
            return "\u001b[31mO\u001b[0m";
        }
        else
        {
            // Números normais (branco)
            return caractere.ToString();
        }
    }
}