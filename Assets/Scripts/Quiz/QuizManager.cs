﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private int score = 0;
    [SerializeField] private int scoreIncrease = 50;
    [SerializeField] private int dificulty;
    [Tooltip("Quantidade total de perguntas que serão realizadas (máximo igual ao total de perguntas)")]
    [SerializeField] private int qtyQuestionsToDo;
    [SerializeField] private int qtyQuestionsDone;
    [Tooltip("ScriptableObject que contém as perguntas de uma determinada dificuldade")]
    public Perguntas[] questionGroup;

    public static QuizManager instance;
    [SerializeField] private List<QuestionAndAnswer> questionAndAnswer;
    [SerializeField] private List<int> numbersList;

    [Tooltip("GameObject que conterá o texto da pergunta")]
    public TextMeshProUGUI questionMeshText;
    [Tooltip("GameObject que conterá o texto da alternativa")]
    public TextMeshProUGUI[] answerMeshText;

    private int index;
    private int questionSelected;
    private int numberOfAnswers;

    #region Set/Get das variáveis
    public void SetDificulty(int value)
    {
        dificulty = value;
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        /// Cria uma lista de perguntas realizadas e respostas dadas pelo player
        questionAndAnswer = new List<QuestionAndAnswer>();

        /// Verifica se o número de perguntas a serem feitas é menor que o numero de perguntas existentes
        /// Caso seja maior, iguala ao total de perguntas
        if (qtyQuestionsToDo > questionGroup[dificulty].GetLenght())
        {
            qtyQuestionsToDo = questionGroup[dificulty].GetLenght();
        }

        /// Inicia a quantidade de questões feitas em zero
        qtyQuestionsDone = 0;

        /// Verifica quantas alternativas existem para cada pergunta
        numberOfAnswers = questionGroup[dificulty].GetQuestion(0).GetNumberOfAlternatives();

        /// Reinicia a lista de valores que podem ser sorteados
        RestartNumberList();

        /// Prepara uma nova pergunta para ser exibida
        PrepareNewQuestion();
    }

    /// <summary>
    /// Função que prepara a nova pergunta que será utilizada no quiz. Responsável por selecionar a pergunta que será utilizada da 
    /// lista e de adicionar na lista de perguntas que já foram utilizadas.
    /// </summary>
    public void PrepareNewQuestion()
    {
        /// Pega o indice da última posição da lista de perguntas e respostas já realizadas
        index = questionAndAnswer.Count;

        /// Adiciona uma nova posição na lista
        questionAndAnswer.Add(new QuestionAndAnswer());
        /// Salva a dificuldade da pergunta a ser feita
        questionAndAnswer[index].SetDificultyLevel(dificulty);

        /// Sorteia a pergunta dentre as possíveis da lista
        questionSelected = RandomQuestionNumber();
        /// Salva qual a pergunta que será realizada
        questionAndAnswer[index].SetQuestionNumber(questionSelected);

        Debug.Log("Questão selecionada foi: " + questionSelected);

        /// Mostra a pergunta na tela
        ShowNewQuestion();
    }

    /// <summary>
    /// Função que mostra a última pergunta sorteada na tela
    /// </summary>
    public void ShowNewQuestion()
    {
        /// Pega a pergunta que deve ser exibida da lista de perguntas
        Pergunta selectedQuestion = questionGroup[dificulty].GetQuestion(questionAndAnswer[index].GetQuestionNumber());
        /// Mostra a pergunta
        questionMeshText.text = selectedQuestion.GetQuestion().text;

        /// Mostra todas as alternativas
        for (int i = 0; i < numberOfAnswers; i++)
        {
            answerMeshText[i].text = selectedQuestion.GetAlternative(i).text;
        }
    }

    /// <summary>
    /// Função que verifica se a resposta selecionada está correta e decide o que deve ser feito
    /// </summary>
    /// <param name="value"></param>
    public void CheckAnswer(int value)
    {
        /// Incrementa o contador de perguntas feitas
        qtyQuestionsDone++;
        /// Salva a alternativa escolhida
        questionAndAnswer[index].SetAnswerSelected(value);
        /// Verifica se a alternativa é a correta
        bool isCorrect = questionGroup[dificulty].GetQuestion(questionSelected).VerifyAnswer(value);

        /// Se for correta, incrementa a pontuação
        if (isCorrect)
        {
            score += scoreIncrease;
        }

        /// Verifica se há mais questões a serem feitas
        if (qtyQuestionsDone != qtyQuestionsToDo)
        {
            /// Se existem, prepara uma nova
            PrepareNewQuestion();
        }
        else
        {
            /// Caso contrário, informa que o quiz acabou
            EndQuiz();
        }
    }

    /// <summary>
    /// Função responsável pelas ações que devem ser realizadas quando o quiz acaba
    /// </summary>
    public void EndQuiz()
    {
        Debug.Log("Quiz Cabo !!!!");
    }

    #region Funções Auxiliares
    /// <summary>
    /// Função que seleciona um número aleatório da lista de perguntas que ainda não foram realizadas
    /// </summary>
    /// <returns></returns>
    public int RandomQuestionNumber()
    {
        /// Seleciona o número aleatório
        int numberSelected = numbersList[Random.Range(0, numbersList.Count)];
        /// Remove da lisata para não ser selecionado novamente
        numbersList.Remove(numberSelected);
        /// Retorna o valor selecionado
        return numberSelected;
    }
    
    /// <summary>
    /// Função que reseta a lista de questões que podem ser utilizadas. Esta lista contém o número das queestões apenas
    /// </summary>
    private void RestartNumberList()
    {
        /// Cria uma nova lista
        numbersList = new List<int>();

        /// Completa a lista com todas as opções possíveis
        for (int i = 0; i < questionGroup[dificulty].GetLenght(); i++)
        {
            numbersList.Add(i);
        }
    }

    #endregion
}