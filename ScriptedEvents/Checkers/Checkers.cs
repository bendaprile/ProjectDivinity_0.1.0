using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Checkers : MonoBehaviour
{
    [SerializeField] int depth = 5;

    CheckersInterface CI;

    public struct MoveData
    {
        public MoveData(int r, int c)
        {
            row = r;
            col = c;
        }

        public int row;
        public int col;
    }
    public struct AIMoveData
    {
        public AIMoveData(int row_in, int col_in)
        {
            move_loc = new MoveData(row_in, col_in);
            kill_locs = new List<(Piece, MoveData)>();
        }

        public MoveData move_loc;
        public List<(Piece, MoveData)> kill_locs;
    }
    public struct AIMoveDataFull
    {
        public MoveData original_loc;
        public AIMoveData move_data;
    }
    public enum Piece { NoPiece, Player, PlayerKing, AI, AIKing}
    public Piece[,] Board = new Piece[8,8];

    AIMoveDataFull best_move;

    void Start()
    {
        CI = GetComponentInChildren<CheckersInterface>();
        CI.SlaveStart();

        Piece PlacementPiece = Piece.NoPiece;
        //set_column_destroy();


        for (int row = 0; row < 8; row++)
        {
            if(row == 0)
            {
                PlacementPiece = Piece.Player;
            }
            else if(row == 3)
            {
                PlacementPiece = Piece.NoPiece;
            }
            else if(row == 5)
            {
                PlacementPiece = Piece.AI;
            }

            bool place_square = (row % 2 == 0);

            for (int column = 0; column < 8; column++)
            {
                if (place_square)
                {
                    MoveData MD = new MoveData();
                    MD.row = row;
                    MD.col = column;

                    Board[row, column] = PlacementPiece;
                }
                place_square = !place_square;
            }
        }


        /*
        for (int row = 0; row < 8; row++)
        {
            for (int column = 0; column < 8; column++)
            {
                Board[row, column] = Piece.NoPiece;
            }
        }

        //Board[2, 0] = Piece.Player;
        Board[1, 1] = Piece.Player;
        Board[3, 1] = Piece.Player;
        //Board[0, 2] = Piece.Player;
        Board[2, 2] = Piece.Player;
        //Board[0, 4] = Piece.Player;
        Board[1, 5] = Piece.Player;
        Board[3, 5] = Piece.Player;
        //Board[0, 6] = Piece.Player;
        //Board[2, 6] = Piece.Player;
        //Board[1, 7] = Piece.Player;

        Board[3, 3] = Piece.AI;
        Board[4, 0] = Piece.AI;
        Board[5, 7] = Piece.AI;
        Board[6, 0] = Piece.AI;
        //Board[6, 2] = Piece.AI;
        //Board[6, 4] = Piece.AI;
        //Board[6, 6] = Piece.AI;
        //Board[7, 1] = Piece.AI;
        //Board[7, 3] = Piece.AI;
        //Board[7, 5] = Piece.AI;
        //Board[7, 7] = Piece.AI;
        */

        CI.SetPieces();
    }


    //////////////////Special logic 
    private const int column_charge_time = 4;

    private int current_charge = -1;
    private int selected_column;

    private void set_column_destroy()
    {
        selected_column = 2;
        current_charge = 0;
    }

    private void update_column_destroy()
    {
        if(current_charge == column_charge_time)
        {
            current_charge = -1;
            column_destroy();
        }
        else if (current_charge != -1)
        {
            current_charge += 1;
        }
    }

    private void column_destroy() //returns all pieces from bottom to top
    {
        Debug.Log("Destroy??");
        for (int row = 0; row < 8; ++row)
        {
            Board[row, selected_column] = Piece.NoPiece;
        }
    }

    private Piece[] check_column_destroy(int current_depth, Piece current_turn, out int points)
    {
        points = 0;
        if (current_charge == -1 || column_charge_time + current_depth != column_charge_time)
        {
            return null;
        }
        else
        {
            Piece[] prev_pieces = new Piece[8];
            for (int row = 0; row < 8; ++row)
            {
                points += kill_point_check(current_turn, Board[row, selected_column]);
                prev_pieces[row] = Board[row, selected_column];
            }
            column_destroy();
            return prev_pieces;
        }
    }
    //////////////////Special logic

   
    public int king_check(AIMoveData AI_MD, Piece exact_type)
    {
        if(AI_MD.move_loc.row == 0 && exact_type == Piece.AI) //Not kings
        {
            Board[AI_MD.move_loc.row, AI_MD.move_loc.col] = Piece.AIKing;
            return 1;
        }
        else if (AI_MD.move_loc.row == 7 && exact_type == Piece.Player) //Not kings
        {
            Board[AI_MD.move_loc.row, AI_MD.move_loc.col] = Piece.PlayerKing;
            return 1;
        }

        return 0;
    }
    
    private bool inbounds(int row, int col)
    {
        return (row >= 0 && row < 8 && col >= 0 && col < 8);
    }

    private bool Hostile_Check(Piece piece1, Piece piece2)
    {
        if(piece1 == Piece.Player || piece1 == Piece.PlayerKing)
        {
            if(piece2 == Piece.AI || piece2 == Piece.AIKing)
            {
                return true;
            }
        }
        else if (piece1 == Piece.AI || piece1 == Piece.AIKing)
        {
            if (piece2 == Piece.Player || piece2 == Piece.PlayerKing)
            {
                return true;
            }
        }
        return false;
    }

    private bool friendly_check(Piece piece1, Piece piece2)
    {
        if (piece1 == Piece.Player || piece1 == Piece.PlayerKing)
        {
            if (piece2 == Piece.Player || piece2 == Piece.PlayerKing)
            {
                return true;
            }
        }
        else if (piece1 == Piece.AI || piece1 == Piece.AIKing)
        {
            if (piece2 == Piece.AI || piece2 == Piece.AIKing)
            {
                return true;
            }
        }
        return false;
    }

    private int kill_point_check(Piece CurrentTurn, Piece check)
    {
        int temp_points = 0;
        if(check == Piece.AI) //AI perspective
        {
            temp_points = -1;
        }
        else if (check == Piece.Player)
        {
            temp_points = 1;
        }
        else if (check == Piece.AIKing)
        {
            temp_points = -2;
        }
        else if(check == Piece.PlayerKing)
        {
            temp_points = 2;
        }

        return (CurrentTurn == Piece.AI) ? temp_points : -temp_points;
    }

    private void return_extra_jumps(Piece exact_type, AIMoveData prev_data, ref List<AIMoveData> valid_moves)
    {
        List<AIMoveData> moves = new List<AIMoveData>();

        for (int type_mult = -1; type_mult <= 1; type_mult += 2)
        {
            if (type_mult == -1 && exact_type == Piece.Player) //Kings ignore this
            {
                continue;
            }
            else if (type_mult == 1 && exact_type == Piece.AI) //Kings ignore this
            {
                continue;
            }

            for (int direction = -1; direction <= 1; direction += 2) // -1(left), 1(right)
            {
                int row_temp = prev_data.move_loc.row + (type_mult);
                int col_temp = prev_data.move_loc.col + (direction);
                int row_temp2 = prev_data.move_loc.row + (type_mult * 2);
                int col_temp2 = prev_data.move_loc.col + (direction * 2);

                if (inbounds(row_temp, col_temp) && inbounds(row_temp2, col_temp2))
                {
                    bool proper_pieces = Hostile_Check(exact_type, Board[row_temp, col_temp]) && Board[row_temp2, col_temp2] == Piece.NoPiece;
                    bool no_repeats = true;
                    foreach ((Piece, MoveData) MDK in prev_data.kill_locs)
                    {
                        if (MDK.Item2.row == row_temp && MDK.Item2.col == col_temp)
                        {
                            no_repeats = false;
                        }
                    }

                    if (proper_pieces && no_repeats)
                    {
                        AIMoveData AMD = new AIMoveData(row_temp2, col_temp2);
                        MoveData temp_kill = new MoveData(row_temp, col_temp);

                        AMD.kill_locs.Add((Board[row_temp, col_temp], temp_kill));
                        foreach ((Piece, MoveData) prev_kill in prev_data.kill_locs) //TODO I dont know if this is a deep copy or if that would be an issue
                        {
                            AMD.kill_locs.Add(prev_kill);
                        }

                        valid_moves.Add(AMD);
                        return_extra_jumps(exact_type, AMD, ref valid_moves);
                    }
                }
            }
        }
    }

    public void return_valid_moves_AI(Piece exact_type, MoveData pos, ref List<AIMoveData> valid_moves)
    {
        //Debug.Log(exact_type);
        for(int type_mult = -1; type_mult <= 1; type_mult += 2)
        {
            if(type_mult == -1 && exact_type == Piece.Player) //Kings ignore this
            {
                continue;
            }
            else if(type_mult == 1 && exact_type == Piece.AI) //Kings ignore this
            {
                continue;
            }

            for (int direction = -1; direction <= 1; direction += 2) // -1(left), 1(right)
            {
                for (int distance = 1; distance <= 2; ++distance) // 1(normal), 2(jump)
                {
                    int row_temp = pos.row + (type_mult * distance);
                    int col_temp = pos.col + (direction * distance);

                    if (inbounds(row_temp, col_temp))
                    {
                        if (Board[row_temp, col_temp] == Piece.NoPiece)
                        {
                            AIMoveData AMD = new AIMoveData(row_temp, col_temp);

                            if (distance == 2)
                            {
                                MoveData kill_temp = new MoveData(pos.row + type_mult, pos.col + direction);
                                AMD.kill_locs.Add((Board[kill_temp.row, kill_temp.col], kill_temp));
                                return_extra_jumps(exact_type, AMD, ref valid_moves);
                            }

                            valid_moves.Add(AMD);
                        }
                        else if (Hostile_Check(exact_type, Board[row_temp, col_temp]))
                        {
                            continue;
                        }
                    }
                    break; //Only continue if the adjacent piece is a Player piece
                }
            }
        }
    }


    
    private void debug(Piece[,] temp_Board)
    {
        Debug.Log("--------");
        for (int row = 7; row >= 0; --row)
        {
            string s = "";
            for (int column = 0; column < 8; column++)
            {
                if (temp_Board[row, column] == Piece.NoPiece)
                {
                    s += " O ";
                }
                else if (temp_Board[row, column] == Piece.Player)
                {
                    s += " P ";
                }
                else if (temp_Board[row, column] == Piece.PlayerKing)
                {
                    s += " K ";
                }
                else if (temp_Board[row, column] == Piece.AI)
                {
                    s += " A ";
                }
                else if (temp_Board[row, column] == Piece.AIKing)
                {
                    s += " U ";
                }
                else
                {
                    s += " - ";
                }
            }

            Debug.Log(s);
        }
        Debug.Log("--------");
    }

    private Piece[,] board_copy()
    {
        Piece[,] new_Board = new Piece[8, 8];
        for(int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                new_Board[i, j] = Board[i, j];
            }
        }
        return new_Board;
    } //DEBUG ONLY

    private void board_check(Piece[,] bc) //DEBUG ONLY
    {
        for (int i = 0; i < 8; ++i)
        {
            for (int j = 0; j < 8; ++j)
            {
                if (bc[i, j] != Board[i, j])
                {
                    debug(bc);
                    debug(Board);
                    Assert.IsTrue(false);

                }
            }
        }
    }
    

    public void start_turn()
    {
        int points = turn_AI(0);

        if(points != -1000)
        {
            Set(best_move, Board[best_move.original_loc.row, best_move.original_loc.col]);
            king_check(best_move.move_data, Checkers.Piece.AI); //Doesn't matter if I put AI or AIking here
        }

        update_column_destroy();
        CI.SetPieces();
    }

    private int turn_AI(int current_depth)
    {
        if(current_depth == depth)
        {
            return 0;
        }

        Piece current_type = (current_depth % 2 == 0) ? Piece.AI : Piece.Player;
        int most_points_current = -1000;

        for (int row = 0; row < 8; ++row)
        {
            for (int col = 0; col < 8; ++col)
            {

                if (friendly_check(current_type, Board[row, col])) //Get all of the pieces of current type
                {
                    Piece original_piece = Board[row, col]; //Piece before any promotion
                    AIMoveDataFull MDF_temp = new AIMoveDataFull();
                    MDF_temp.original_loc = new MoveData(row, col);
                    List<AIMoveData> valid_moves = new List<AIMoveData>();
                        
                    return_valid_moves_AI(Board[row, col], MDF_temp.original_loc, ref valid_moves); //All moves for one piece

                    for (int j = 0; j < valid_moves.Count; ++j)
                    {
                        MDF_temp.move_data = valid_moves[j];
                        int local_points = MDF_temp.move_data.kill_locs.Count;

                        /*
                        //DEBUG 
                        string Space = "";
                        if (current_depth == 2)
                        {
                            Space = "                      ";
                        }
                        else if (current_depth == 1)
                        {
                            Space = "          ";
                        }
            
                        Debug.Log((Space, "Local Points: ", local_points, "  ", Board[MDF_temp.original_loc.row, MDF_temp.original_loc.col], MDF_temp.original_loc.row, MDF_temp.original_loc.col, "Move Loc: ", MDF_temp.move_data.move_loc.row, MDF_temp.move_data.move_loc.col));
                        //DEBUG
                        */

                        //Piece[,] check = board_copy();
                        


                        //SET////
                        Set(MDF_temp, original_piece);
                        local_points += king_check(valid_moves[j], original_piece); //overrides the piece
                        ////////


                        ///SPECIALSET
                        int points_temp_special;
                        Piece[] special_row = check_column_destroy(current_depth, Board[row, col], out points_temp_special);
                        local_points += points_temp_special;
                        ///SPECIALSET


                        int points_temp = turn_AI(current_depth + 1);
                        points_temp = (points_temp > -500) ? points_temp : -500; 
                        local_points -= points_temp;


                        ///SPECIALRESET
                        if (special_row != null)
                        {
                            for (int spec_row_iter = 0; spec_row_iter < 8; ++spec_row_iter)
                            {
                                Board[spec_row_iter, selected_column] = special_row[spec_row_iter];
                            }
                        }
                        ///SPECIALRESET

                        //RESET/////
                        Reset(MDF_temp, original_piece);
                        ////////////
                        


                        //board_check(check);


                        if (local_points > most_points_current)
                        {
                            most_points_current = local_points;
                            if(current_depth == 0)
                            {
                                best_move = MDF_temp;
                            }
                        }
                    }
                }
            }
        }
        return most_points_current;
    }

    public void Set(AIMoveDataFull MDF, Piece exact_type) //Not the exact piece
    {
        Board[MDF.original_loc.row, MDF.original_loc.col] = Piece.NoPiece;
        Board[MDF.move_data.move_loc.row, MDF.move_data.move_loc.col] = exact_type;
        
        for(int i = 0; i < MDF.move_data.kill_locs.Count; ++i)
        {
            Board[MDF.move_data.kill_locs[i].Item2.row, MDF.move_data.kill_locs[i].Item2.col] = Piece.NoPiece;
        }
    }

    private void Reset(AIMoveDataFull MDF, Piece original_type) //Piece before promotion
    {
        //Debug.Log(original_type);
        Board[MDF.original_loc.row, MDF.original_loc.col] = original_type;
        Board[MDF.move_data.move_loc.row, MDF.move_data.move_loc.col] = Piece.NoPiece;

        for (int i = 0; i < MDF.move_data.kill_locs.Count; ++i)
        {
            Board[MDF.move_data.kill_locs[i].Item2.row, MDF.move_data.kill_locs[i].Item2.col] = MDF.move_data.kill_locs[i].Item1;
        }
    }
}
