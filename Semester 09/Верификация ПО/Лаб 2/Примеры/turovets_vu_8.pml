#define N 8

#define TOP(i)   sticks[i].stack[sticks[i].count - 1]
#define EMPTY(i) (sticks[i].count == 0)

typedef Stick {
  byte stack[N];
  byte count;
};

Stick sticks[3];

inline CanMove(i, j) {
  !EMPTY(i) && (EMPTY(j) || TOP(i) < TOP(j))
}

inline Move(i, j) {
  atomic {
    printf("(%d, %d) -> (%d)\n", i, TOP(i), j);
    sticks[j].count++; TOP(j) = TOP(i); sticks[i].count--;
  }
}

proctype main() {
  do
  :: CanMove(0, 1) -> Move(0, 1);
  :: CanMove(0, 2) -> Move(0, 2);
  :: CanMove(1, 0) -> Move(1, 0);
  :: CanMove(1, 2) -> Move(1, 2);
  :: CanMove(2, 0) -> Move(2, 0);
  :: CanMove(2, 1) -> Move(2, 1);
  od
}

init {
  int i;

  for (i: 0 .. (N - 1)) {
    sticks[0].stack[i] = (N - i);
  }

  sticks[0].count = N;
  sticks[1].count = 0;
  sticks[2].count = 0;

  run main();
}

ltl {
  !<>(sticks[2].count == N)
}
