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

    static void Main(string[] args)
    {
        bool sairDoJogo = false;

        while (!sairDoJogo)
        {
            Console.Clear();
            Console.WriteLine("=== Jogo da Velha ===");
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

            modoVsComputador = (opcaoMenu == 2);

            if (modoVsComputador)
            {
                Console.Clear();
                Console.WriteLine("=== Dificuldade da IA ===");
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
                Console.WriteLine(modoVsComputador ?
                    $"Modo: Jogador vs Computador (Dificuldade: {(dificuldadeIA == 1 ? "Fácil" : "Difícil")}" :
                    "Modo: Jogador vs Jogador");

                do
                {
                    Console.Clear();
                    Console.WriteLine(modoVsComputador ?
                        "Jogador: X | Computador: O" :
                        "Jogador 1: X | Jogador 2: O");
                    Console.WriteLine();

                    DesenharTabuleiro();

                    if (vencedor == 0 && !ExistemJogadasDisponiveis())
                    {
                        Console.WriteLine("\nEmpate!");
                        break;
                    }

                    if (vencedor != 0)
                    {
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
                        break;
                    }

                    if (jogadorAtual == 1 || !modoVsComputador)
                    {
                        Console.WriteLine($"\n{(modoVsComputador ? "Sua vez" : $"Vez do Jogador {jogadorAtual}")}. Escolha uma posição (1-9): ");

                        bool conversaoValida = int.TryParse(Console.ReadLine(), out escolha);

                        if (conversaoValida && escolha >= 1 && escolha <= 9 && tabuleiro[escolha - 1] != 'X' && tabuleiro[escolha - 1] != 'O')
                        {
                            char marca = (jogadorAtual == 1) ? 'X' : 'O';
                            tabuleiro[escolha - 1] = marca;

                            if (VerificarVencedor(marca))
                            {
                                vencedor = jogadorAtual;
                            }
                            else
                            {
                                if (modoVsComputador)
                                {
                                    jogadorAtual = 2;

                                    if (vencedor == 0 && ExistemJogadasDisponiveis())
                                    {
                                        JogadaComputador();
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
                        JogadaComputador();
                    }

                } while (true);

                Console.WriteLine("\nO que deseja fazer agora?");
                Console.WriteLine("1 - Jogar novamente no mesmo modo");
                Console.WriteLine("2 - Voltar ao menu principal");
                Console.WriteLine("3 - Sair do jogo");
                Console.Write("\nEscolha uma opção: ");

                int opcaoPosJogo;
                while (!int.TryParse(Console.ReadLine(), out opcaoPosJogo) || (opcaoPosJogo < 1 || opcaoPosJogo > 3))
                {
                    Console.Write("Opção inválida. Digite 1, 2 ou 3: ");
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
                }
            }
        }

        Console.WriteLine("\nObrigado por jogar! Até a próxima!");
    }

    private static void InicializarJogo()
    {
        tabuleiro = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        jogadorAtual = 1;
        vencedor = 0;
    }

    private static void JogadaComputador()
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

        Console.WriteLine($"\nO computador jogou na posição {jogada + 1}");
        System.Threading.Thread.Sleep(1000); // Pequena pausa para o jogador ver a jogada

        if (VerificarVencedor('O'))
        {
            vencedor = 2;
        }
        else
        {
            jogadorAtual = 1;
        }
    }

    private static int JogadaFacil()
    {
        // Jogada totalmente aleatória
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
        // 1. Tenta vencer se possível
        int jogadaVencedora = EncontrarJogadaVencedora('O');
        if (jogadaVencedora != -1)
        {
            tabuleiro[jogadaVencedora] = 'O';
            return jogadaVencedora;
        }

        // 2. Tenta bloquear o jogador
        int jogadaBloqueio = EncontrarJogadaVencedora('X');
        if (jogadaBloqueio != -1)
        {
            tabuleiro[jogadaBloqueio] = 'O';
            return jogadaBloqueio;
        }

        // 3. Estratégia avançada
        int[] posicoesPrioritarias = { 4, 0, 2, 6, 8, 1, 3, 5, 7 };

        foreach (int posicao in posicoesPrioritarias)
        {
            if (tabuleiro[posicao] != 'X' && tabuleiro[posicao] != 'O')
            {
                tabuleiro[posicao] = 'O';
                return posicao;
            }
        }

        return -1; // Nunca deve chegar aqui
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
        // Verifica linhas
        if ((tabuleiro[0] == marca && tabuleiro[1] == marca && tabuleiro[2] == marca) ||
            (tabuleiro[3] == marca && tabuleiro[4] == marca && tabuleiro[5] == marca) ||
            (tabuleiro[6] == marca && tabuleiro[7] == marca && tabuleiro[8] == marca) ||
            // Verifica colunas
            (tabuleiro[0] == marca && tabuleiro[3] == marca && tabuleiro[6] == marca) ||
            (tabuleiro[1] == marca && tabuleiro[4] == marca && tabuleiro[7] == marca) ||
            (tabuleiro[2] == marca && tabuleiro[5] == marca && tabuleiro[8] == marca) ||
            // Verifica diagonais
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