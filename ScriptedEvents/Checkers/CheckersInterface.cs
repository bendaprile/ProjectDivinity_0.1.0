using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersInterface : MonoBehaviour
{
    [SerializeField] Transform PlayerObject;
    [SerializeField] Transform AIObject;
    [SerializeField] Transform PlayerKingObject;
    [SerializeField] Transform AIKingObject;
    [SerializeField] Transform ValidMoveIndicator;
    [SerializeField] Transform SelectedIndicator;

    [SerializeField] private Transform bottomLeftTrans;
    [SerializeField] private float scale = 2.5f;

    private Vector3 bottomLeft;
    private Checkers check;

    private Transform Pieces;
    private Transform Indicators;


    private bool is_piece_queued = false;
    private Checkers.MoveData piece_queued;
    List<Checkers.AIMoveData> validMoves = new List<Checkers.AIMoveData>();

    public void SlaveStart() //Have this called by Checkers so there isn't a race condition
    {
        bottomLeft = bottomLeftTrans.position;
        check = GetComponentInParent<Checkers>();

        Pieces = GameObject.Find("Pieces").transform;
        Indicators = GameObject.Find("Indicators").transform;
    }

    private Checkers.MoveData return_square(Vector3 point) //Returns in row, col format
    {
        Vector3 diff_vec = point - bottomLeft;
        int row = (int)((diff_vec.x + diff_vec.z) / (Mathf.Sqrt(2) * scale));
        int col = (int)((diff_vec.x - diff_vec.z) / (Mathf.Sqrt(2) * scale));
        //int row = (int)(diff_vec.z / scale);
        //int col = (int)(diff_vec.x / scale);

        return new Checkers.MoveData(row, col);
    }

    public void SetPieces()
    {
        foreach (Transform child in Pieces)
        {
            Destroy(child.gameObject);
        }

        for (int row = 7; row >= 0; --row)
        {
            for (int column = 0; column < 8; column++)
            {
                if (check.Board[row, column] == Checkers.Piece.Player)
                {
                    Transform temp = Instantiate(PlayerObject, Pieces).transform;
                    temp.localPosition = new Vector3(column * scale, 0, row * scale);
                }
                else if (check.Board[row, column] == Checkers.Piece.PlayerKing)
                {
                    Transform temp = Instantiate(PlayerKingObject, Pieces).transform;
                    temp.localPosition = new Vector3(column * scale, 0, row * scale);
                }
                else if (check.Board[row, column] == Checkers.Piece.AI)
                {
                    Transform temp = Instantiate(AIObject, Pieces).transform;
                    temp.localPosition = new Vector3(column * scale, 0, row * scale);
                }
                else if (check.Board[row, column] == Checkers.Piece.AIKing)
                {
                    Transform temp = Instantiate(AIKingObject, Pieces).transform;
                    temp.localPosition = new Vector3(column * scale, 0, row * scale);
                }
            }
        }
    }

    private void SetIndicators()
    {
        Transform temp = Instantiate(SelectedIndicator, Indicators).transform;
        temp.localPosition = new Vector3(piece_queued.col * scale, 0, piece_queued.row * scale);

        for (int i = 0; i < validMoves.Count; ++i)
        {
            temp = Instantiate(ValidMoveIndicator, Indicators).transform;
            temp.localPosition = new Vector3(validMoves[i].move_loc.col * scale, 0, validMoves[i].move_loc.row * scale);
        }
    }

    private void DestroyIndicators()
    {
        validMoves.Clear();
        foreach (Transform child in Indicators)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitray;

            if (Physics.Raycast(ray, out hitray, Mathf.Infinity, (LayerMask.GetMask("ScriptedEventLayer"))))
            {
                Vector3 contact = hitray.point;
                Checkers.MoveData square = return_square(contact);

                //Debug.Log((square.row, square.col));

                if (check.Board[square.row, square.col] == Checkers.Piece.Player || check.Board[square.row, square.col] == Checkers.Piece.PlayerKing)
                {
                    is_piece_queued = true;
                    piece_queued = square;
                    DestroyIndicators();
                    check.return_valid_moves_AI(check.Board[piece_queued.row, piece_queued.col], piece_queued, ref validMoves);
                    SetIndicators();
                }
                else if (is_piece_queued)
                {
                    for (int i = 0; i < validMoves.Count; ++i)
                    {
                        if (validMoves[i].move_loc.row == square.row && validMoves[i].move_loc.col == square.col)
                        {
                            is_piece_queued = false;

                            Checkers.AIMoveDataFull MDF_player = new Checkers.AIMoveDataFull();
                            MDF_player.original_loc = piece_queued;
                            MDF_player.move_data = validMoves[i];

                            check.Set(MDF_player, check.Board[piece_queued.row, piece_queued.col]);
                            check.king_check(MDF_player.move_data, Checkers.Piece.Player); //Doesn't matter if I put Player or Playerking here
                            check.start_turn();
                            DestroyIndicators();
                            break;
                        }
                    }
                }
            }
        }
    }
}
