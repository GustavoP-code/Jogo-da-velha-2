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

                    if (vencedor == 0 && !ExistemJogadasDisponiveis())
                    {
                        Console.WriteLine("\nEmpate!");
                        empates++;
                        break;
                    }

                    if (vencedor != 0)
                    {
                        if (modoVsComputador)
                        {
                            if (vencedor == 1)
                            {
                                Console.WriteLine("\nParabéns! Você venceu!");
                                pontuacaoJogador1++;
                            }
                            else
                            {
                                Console.WriteLine("\nO computador venceu!");
                                pontuacaoComputador++;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\nJogador {vencedor} venceu!");
                            if (vencedor == 1) pontuacaoJogador1++;
                            else pontuacaoJogador2++;
                        }
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

                                // Atualiza pontuação e mostra placar atualizado
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

                        if (vencedor == 0 && ExistemJogadasDisponiveis())
                        {
                            jogadorAtual = 1; // Volta para o jogador humano
                            primeiraJogadaHumano = false; // Não é mais a primeira jogada
                        }
                        else if (vencedor == 2)
                        {
                            // Computador venceu, atualiza pontuação
                            pontuacaoComputador++;
                            Console.Clear();
                            MostrarPlacarCompleto();
                            Console.WriteLine("\nJogador: X | Computador: O");
                            Console.WriteLine();
                            DesenharTabuleiro();
                            Console.WriteLine("\nO computador venceu!");
                        }
                    }

                } while (vencedor == 0 && ExistemJogadasDisponiveis());

                // Se houve empate, mostra a tela final
                if (vencedor == 0 && !ExistemJogadasDisponiveis())
                {
                    Console.Clear();
                    MostrarPlacarCompleto();
                    Console.WriteLine(modoVsComputador ?
                        "\nJogador: X | Computador: O" :
                        "\nJogador 1: X | Jogador 2: O");
                    Console.WriteLine();
                    DesenharTabuleiro();
                    Console.WriteLine("\nEmpate!");
                    empates++;
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
        // Removida a mensagem de confirmação
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

        if (VerificarVencedor('O'))
        {
            vencedor = 2;
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
        int jogadaVencedora = EncontrarJogadaVencedora('O');
        if (jogadaVencedora != -1)
        {
            tabuleiro[jogadaVencedora] = 'O';
            return jogadaVencedora;
        }

        int jogadaBloqueio = EncontrarJogadaVencedora('X');
        if (jogadaBloqueio != -1)
        {
            tabuleiro[jogadaBloqueio] = 'O';
            return jogadaBloqueio;
        }

        int[] posicoesPrioritarias = { 4, 0, 2, 6, 8, 1, 3, 5, 7 };

        foreach (int posicao in posicoesPrioritarias)
        {
            if (tabuleiro[posicao] != 'X' && tabuleiro[posicao] != 'O')
            {
                tabuleiro[posicao] = 'O';
                return posicao;
            }
        }

        return -1;
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

    private static void DesenharTabuleiro()
    {
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {tabuleiro[0]}  |  {tabuleiro[1]}  |  {tabuleiro[2]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {tabuleiro[3]}  |  {tabuleiro[4]}  |  {tabuleiro[5]}  ");
        Console.WriteLine("_____|_____|_____");
        Console.WriteLine("     |     |     ");
        Console.WriteLine($"  {tabuleiro[6]}  |  {tabuleiro[7]}  |  {tabuleiro[8]}  ");
        Console.WriteLine("     |     |     ");
    }

    private static bool VerificarVencedor(char marca)
    {
        if ((tabuleiro[0] == marca && tabuleiro[1] == marca && tabuleiro[2] == marca) ||
            (tabuleiro[3] == marca && tabuleiro[4] == marca && tabuleiro[5] == marca) ||
            (tabuleiro[6] == marca && tabuleiro[7] == marca && tabuleiro[8] == marca) ||
            (tabuleiro[0] == marca && tabuleiro[3] == marca && tabuleiro[6] == marca) ||
            (tabuleiro[1] == marca && tabuleiro[4] == marca && tabuleiro[7] == marca) ||
            (tabuleiro[2] == marca && tabuleiro[5] == marca && tabuleiro[8] == marca) ||
            (tabuleiro[0] == marca && tabuleiro[4] == marca && tabuleiro[8] == marca) ||
            (tabuleiro[2] == marca && tabuleiro[4] == marca && tabuleiro[6] == marca))
        {
            return true;
        }
        return false;
    }

    private static bool ExistemJogadasDisponiveis()
    {
        foreach (char posicao in tabuleiro)
        {
            if (posicao != 'X' && posicao != 'O')
            {
                return true;
            }
        }
        return false;
    }
}