#define N 10
#define E 0
#define B 1
#define W 2

#define PAIR(i) (board[i] != E && board[(i)+1] != E)
#define EMPTY_PAIR(j) (board[j] == E && board[(j)+1] == E)
#define GOAL (board[0] == W && board[1] == W && board[2] == W && board[3] == B && board[4] == B && board[5] == B && board[6] == E && board[7] == E && board[8] == E && board[9] == E)
#define GOAL_AT_3 (GOAL && steps == 3)

byte board[N];
byte steps;

inline print_cell(idx) {
  if
  :: board[idx] == E -> printf("_")
  :: board[idx] == B -> printf("B")
  :: board[idx] == W -> printf("W")
  fi
}

inline print_board() {
  byte k;
  printf("board=");
  k = 0;
  do
  :: k < N ->
       print_cell(k);
       k++;
  :: else -> break
  od;
  printf("\n");
}

inline do_move(ii, jj) {
  byte a;
  byte b;
  atomic {
    a = board[ii];
    b = board[(ii)+1];
    board[ii] = E;
    board[(ii)+1] = E;
    board[jj] = a;
    board[(jj)+1] = b;
    steps++;
  }
}

inline report_move(ii, jj) {
  printf("step %d: i=%d j=%d ", steps, ii, jj);
  print_board();
}

inline move_once() {
  if
  :: (PAIR(0) && EMPTY_PAIR(0) && steps < 3) -> do_move(0, 0); report_move(0, 0);
  :: (PAIR(0) && EMPTY_PAIR(1) && steps < 3) -> do_move(0, 1); report_move(0, 1);
  :: (PAIR(0) && EMPTY_PAIR(2) && steps < 3) -> do_move(0, 2); report_move(0, 2);
  :: (PAIR(0) && EMPTY_PAIR(3) && steps < 3) -> do_move(0, 3); report_move(0, 3);
  :: (PAIR(0) && EMPTY_PAIR(4) && steps < 3) -> do_move(0, 4); report_move(0, 4);
  :: (PAIR(0) && EMPTY_PAIR(5) && steps < 3) -> do_move(0, 5); report_move(0, 5);
  :: (PAIR(0) && EMPTY_PAIR(6) && steps < 3) -> do_move(0, 6); report_move(0, 6);
  :: (PAIR(0) && EMPTY_PAIR(7) && steps < 3) -> do_move(0, 7); report_move(0, 7);
  :: (PAIR(0) && EMPTY_PAIR(8) && steps < 3) -> do_move(0, 8); report_move(0, 8);
  :: (PAIR(1) && EMPTY_PAIR(0) && steps < 3) -> do_move(1, 0); report_move(1, 0);
  :: (PAIR(1) && EMPTY_PAIR(1) && steps < 3) -> do_move(1, 1); report_move(1, 1);
  :: (PAIR(1) && EMPTY_PAIR(2) && steps < 3) -> do_move(1, 2); report_move(1, 2);
  :: (PAIR(1) && EMPTY_PAIR(3) && steps < 3) -> do_move(1, 3); report_move(1, 3);
  :: (PAIR(1) && EMPTY_PAIR(4) && steps < 3) -> do_move(1, 4); report_move(1, 4);
  :: (PAIR(1) && EMPTY_PAIR(5) && steps < 3) -> do_move(1, 5); report_move(1, 5);
  :: (PAIR(1) && EMPTY_PAIR(6) && steps < 3) -> do_move(1, 6); report_move(1, 6);
  :: (PAIR(1) && EMPTY_PAIR(7) && steps < 3) -> do_move(1, 7); report_move(1, 7);
  :: (PAIR(1) && EMPTY_PAIR(8) && steps < 3) -> do_move(1, 8); report_move(1, 8);
  :: (PAIR(2) && EMPTY_PAIR(0) && steps < 3) -> do_move(2, 0); report_move(2, 0);
  :: (PAIR(2) && EMPTY_PAIR(1) && steps < 3) -> do_move(2, 1); report_move(2, 1);
  :: (PAIR(2) && EMPTY_PAIR(2) && steps < 3) -> do_move(2, 2); report_move(2, 2);
  :: (PAIR(2) && EMPTY_PAIR(3) && steps < 3) -> do_move(2, 3); report_move(2, 3);
  :: (PAIR(2) && EMPTY_PAIR(4) && steps < 3) -> do_move(2, 4); report_move(2, 4);
  :: (PAIR(2) && EMPTY_PAIR(5) && steps < 3) -> do_move(2, 5); report_move(2, 5);
  :: (PAIR(2) && EMPTY_PAIR(6) && steps < 3) -> do_move(2, 6); report_move(2, 6);
  :: (PAIR(2) && EMPTY_PAIR(7) && steps < 3) -> do_move(2, 7); report_move(2, 7);
  :: (PAIR(2) && EMPTY_PAIR(8) && steps < 3) -> do_move(2, 8); report_move(2, 8);
  :: (PAIR(3) && EMPTY_PAIR(0) && steps < 3) -> do_move(3, 0); report_move(3, 0);
  :: (PAIR(3) && EMPTY_PAIR(1) && steps < 3) -> do_move(3, 1); report_move(3, 1);
  :: (PAIR(3) && EMPTY_PAIR(2) && steps < 3) -> do_move(3, 2); report_move(3, 2);
  :: (PAIR(3) && EMPTY_PAIR(3) && steps < 3) -> do_move(3, 3); report_move(3, 3);
  :: (PAIR(3) && EMPTY_PAIR(4) && steps < 3) -> do_move(3, 4); report_move(3, 4);
  :: (PAIR(3) && EMPTY_PAIR(5) && steps < 3) -> do_move(3, 5); report_move(3, 5);
  :: (PAIR(3) && EMPTY_PAIR(6) && steps < 3) -> do_move(3, 6); report_move(3, 6);
  :: (PAIR(3) && EMPTY_PAIR(7) && steps < 3) -> do_move(3, 7); report_move(3, 7);
  :: (PAIR(3) && EMPTY_PAIR(8) && steps < 3) -> do_move(3, 8); report_move(3, 8);
  :: (PAIR(4) && EMPTY_PAIR(0) && steps < 3) -> do_move(4, 0); report_move(4, 0);
  :: (PAIR(4) && EMPTY_PAIR(1) && steps < 3) -> do_move(4, 1); report_move(4, 1);
  :: (PAIR(4) && EMPTY_PAIR(2) && steps < 3) -> do_move(4, 2); report_move(4, 2);
  :: (PAIR(4) && EMPTY_PAIR(3) && steps < 3) -> do_move(4, 3); report_move(4, 3);
  :: (PAIR(4) && EMPTY_PAIR(4) && steps < 3) -> do_move(4, 4); report_move(4, 4);
  :: (PAIR(4) && EMPTY_PAIR(5) && steps < 3) -> do_move(4, 5); report_move(4, 5);
  :: (PAIR(4) && EMPTY_PAIR(6) && steps < 3) -> do_move(4, 6); report_move(4, 6);
  :: (PAIR(4) && EMPTY_PAIR(7) && steps < 3) -> do_move(4, 7); report_move(4, 7);
  :: (PAIR(4) && EMPTY_PAIR(8) && steps < 3) -> do_move(4, 8); report_move(4, 8);
  :: (PAIR(5) && EMPTY_PAIR(0) && steps < 3) -> do_move(5, 0); report_move(5, 0);
  :: (PAIR(5) && EMPTY_PAIR(1) && steps < 3) -> do_move(5, 1); report_move(5, 1);
  :: (PAIR(5) && EMPTY_PAIR(2) && steps < 3) -> do_move(5, 2); report_move(5, 2);
  :: (PAIR(5) && EMPTY_PAIR(3) && steps < 3) -> do_move(5, 3); report_move(5, 3);
  :: (PAIR(5) && EMPTY_PAIR(4) && steps < 3) -> do_move(5, 4); report_move(5, 4);
  :: (PAIR(5) && EMPTY_PAIR(5) && steps < 3) -> do_move(5, 5); report_move(5, 5);
  :: (PAIR(5) && EMPTY_PAIR(6) && steps < 3) -> do_move(5, 6); report_move(5, 6);
  :: (PAIR(5) && EMPTY_PAIR(7) && steps < 3) -> do_move(5, 7); report_move(5, 7);
  :: (PAIR(5) && EMPTY_PAIR(8) && steps < 3) -> do_move(5, 8); report_move(5, 8);
  :: (PAIR(6) && EMPTY_PAIR(0) && steps < 3) -> do_move(6, 0); report_move(6, 0);
  :: (PAIR(6) && EMPTY_PAIR(1) && steps < 3) -> do_move(6, 1); report_move(6, 1);
  :: (PAIR(6) && EMPTY_PAIR(2) && steps < 3) -> do_move(6, 2); report_move(6, 2);
  :: (PAIR(6) && EMPTY_PAIR(3) && steps < 3) -> do_move(6, 3); report_move(6, 3);
  :: (PAIR(6) && EMPTY_PAIR(4) && steps < 3) -> do_move(6, 4); report_move(6, 4);
  :: (PAIR(6) && EMPTY_PAIR(5) && steps < 3) -> do_move(6, 5); report_move(6, 5);
  :: (PAIR(6) && EMPTY_PAIR(6) && steps < 3) -> do_move(6, 6); report_move(6, 6);
  :: (PAIR(6) && EMPTY_PAIR(7) && steps < 3) -> do_move(6, 7); report_move(6, 7);
  :: (PAIR(6) && EMPTY_PAIR(8) && steps < 3) -> do_move(6, 8); report_move(6, 8);
  :: (PAIR(7) && EMPTY_PAIR(0) && steps < 3) -> do_move(7, 0); report_move(7, 0);
  :: (PAIR(7) && EMPTY_PAIR(1) && steps < 3) -> do_move(7, 1); report_move(7, 1);
  :: (PAIR(7) && EMPTY_PAIR(2) && steps < 3) -> do_move(7, 2); report_move(7, 2);
  :: (PAIR(7) && EMPTY_PAIR(3) && steps < 3) -> do_move(7, 3); report_move(7, 3);
  :: (PAIR(7) && EMPTY_PAIR(4) && steps < 3) -> do_move(7, 4); report_move(7, 4);
  :: (PAIR(7) && EMPTY_PAIR(5) && steps < 3) -> do_move(7, 5); report_move(7, 5);
  :: (PAIR(7) && EMPTY_PAIR(6) && steps < 3) -> do_move(7, 6); report_move(7, 6);
  :: (PAIR(7) && EMPTY_PAIR(7) && steps < 3) -> do_move(7, 7); report_move(7, 7);
  :: (PAIR(7) && EMPTY_PAIR(8) && steps < 3) -> do_move(7, 8); report_move(7, 8);
  :: (PAIR(8) && EMPTY_PAIR(0) && steps < 3) -> do_move(8, 0); report_move(8, 0);
  :: (PAIR(8) && EMPTY_PAIR(1) && steps < 3) -> do_move(8, 1); report_move(8, 1);
  :: (PAIR(8) && EMPTY_PAIR(2) && steps < 3) -> do_move(8, 2); report_move(8, 2);
  :: (PAIR(8) && EMPTY_PAIR(3) && steps < 3) -> do_move(8, 3); report_move(8, 3);
  :: (PAIR(8) && EMPTY_PAIR(4) && steps < 3) -> do_move(8, 4); report_move(8, 4);
  :: (PAIR(8) && EMPTY_PAIR(5) && steps < 3) -> do_move(8, 5); report_move(8, 5);
  :: (PAIR(8) && EMPTY_PAIR(6) && steps < 3) -> do_move(8, 6); report_move(8, 6);
  :: (PAIR(8) && EMPTY_PAIR(7) && steps < 3) -> do_move(8, 7); report_move(8, 7);
  :: (PAIR(8) && EMPTY_PAIR(8) && steps < 3) -> do_move(8, 8); report_move(8, 8);
  fi
}

proctype main() {
  printf("init ");
  print_board();

  do
  :: steps < 3 -> move_once()
  :: else -> break
  od;

  printf("final ");
  print_board();
}

init {
  board[0] = E; board[1] = E; board[2] = E; board[3] = E;
  board[4] = B; board[5] = W; board[6] = B; board[7] = W; board[8] = B; board[9] = W;
  steps = 0;
  run main();
}

ltl { !<>(GOAL_AT_3) }
