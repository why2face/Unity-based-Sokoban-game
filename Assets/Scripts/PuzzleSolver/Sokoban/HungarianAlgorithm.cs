﻿/**
 * Hungarian Algorithm code by Kevin L. Stern
 * (https://github.com/KevinStern/software-and-algorithms/blob/master/src/main/java/
 * blogspot/software_and_algorithms/stern_library/optimization/HungarianAlgorithm.java)
 * @author Hyun Seung Hong (hh2473)
 *
 */
using System;
public class HungarianAlgorithm {
	private int[,] costMatrix;
	private int rows, cols, dim;
	private int[] labelByWorker, labelByJob;
	private int[] minSlackWorkerByJob;
	private int[] minSlackValueByJob;
	private int[] matchJobByWorker, matchWorkerByJob;
	private int[] parentWorkerByCommittedJob;
	private bool[] committedWorkers;

	/**
	 * Construct an instance of the algorithm.
	 * 
	 * @param costMatrix
	 *            the cost matrix, where matrix[i][j] holds the cost of
	 *            assigning worker i to job j, for all i, j. The cost matrix
	 *            must not be irregular in the sense that all rows must be the
	 *            same length.
	 */
	public HungarianAlgorithm(int matrixSize) {
		this.dim = Math.Max(matrixSize, matrixSize);
		this.rows = matrixSize;
		this.cols = matrixSize;
		this.costMatrix = new int[this.dim,this.dim];
		labelByWorker = new int[this.dim];
		labelByJob = new int[this.dim];
		minSlackWorkerByJob = new int[this.dim];
		minSlackValueByJob = new int[this.dim];
		committedWorkers = new bool[this.dim];
		parentWorkerByCommittedJob = new int[this.dim];
		matchJobByWorker = new int[this.dim];
		matchJobByWorker.Fill(-1);
		matchWorkerByJob = new int[this.dim];
        matchWorkerByJob.Fill(-1);
	}

	/**
	 * Compute an initial feasible solution by assigning zero labels to the
	 * workers and by assigning to each job a label equal to the minimum cost
	 * among its incident edges.
	 */
	protected void computeInitialFeasibleSolution() {
		for (int j = 0; j < dim; j++) {
			labelByJob[j] = int.MaxValue;
		}
		for (int w = 0; w < dim; w++) {
			for (int j = 0; j < dim; j++) {
				if (costMatrix[w,j] < labelByJob[j]) {
					labelByJob[j] = costMatrix[w,j];
				}
			}
		}
	}

	/**
	 * Execute the algorithm.
	 * 
	 * @return the minimum cost matching of workers to jobs based upon the
	 *         provided cost matrix. A matching value of -1 indicates that the
	 *         corresponding worker is unassigned.
	 */
	public int[] execute(int[,] costMatrix) {
        
        matchJobByWorker.Fill(-1);
        matchWorkerByJob.Fill(-1);
        Array.Clear(labelByWorker,0,labelByWorker.Length);
        Array.Clear(labelByJob, 0, labelByJob.Length);
        Array.Clear(minSlackWorkerByJob, 0, minSlackWorkerByJob.Length);
        Array.Clear(minSlackValueByJob, 0, minSlackValueByJob.Length);
        Array.Clear(committedWorkers, 0, committedWorkers.Length);
        Array.Clear(parentWorkerByCommittedJob, 0, parentWorkerByCommittedJob.Length);

		/*
		 * Heuristics to improve performance: Reduce rows and columns by their
		 * smallest element, compute an initial non-zero dual feasible solution
		 * and create a greedy matching from workers to jobs of the cost matrix.
		 */
		this.costMatrix = costMatrix;
		reduce();
		computeInitialFeasibleSolution();
		greedyMatch();

		int w = fetchUnmatchedWorker();
		while (w < dim) {
			initializePhase(w);
			executePhase();
			w = fetchUnmatchedWorker();
		}
        int[] result = new int[rows];
		Array.Copy(matchJobByWorker, result, rows);
		for (w = 0; w < result.Length; w++) {
			if (result[w] >= cols) {
				result[w] = -1;
			}
		}
		return result;
	}

	/**
	 * Execute a single phase of the algorithm. A phase of the Hungarian
	 * algorithm consists of building a set of committed workers and a set of
	 * committed jobs from a root unmatched worker by following alternating
	 * unmatched/matched zero-slack edges. If an unmatched job is encountered,
	 * then an augmenting path has been found and the matching is grown. If the
	 * connected zero-slack edges have been exhausted, the labels of committed
	 * workers are increased by the minimum slack among committed workers and
	 * non-committed jobs to create more zero-slack edges (the labels of
	 * committed jobs are simultaneously decreased by the same amount in order
	 * to maintain a feasible labeling).
	 * <p>
	 * 
	 * The runtime of a single phase of the algorithm is O(n^2), where n is the
	 * dimension of the internal square cost matrix, since each edge is visited
	 * at most once and since increasing the labeling is accomplished in time
	 * O(n) by maintaining the minimum slack values among non-committed jobs.
	 * When a phase completes, the matching will have increased in size.
	 */
	protected void executePhase() {
		while (true) {
			int minSlackWorker = -1, minSlackJob = -1;
            int minSlackValue = int.MaxValue;
			for (int j = 0; j < dim; j++) {
				if (parentWorkerByCommittedJob[j] == -1) {
					if (minSlackValueByJob[j] < minSlackValue) {
						minSlackValue = minSlackValueByJob[j];
						minSlackWorker = minSlackWorkerByJob[j];
						minSlackJob = j;
					}
				}
			}
			if (minSlackValue > 0) {
				updateLabeling(minSlackValue);
			}
			parentWorkerByCommittedJob[minSlackJob] = minSlackWorker;
			if (matchWorkerByJob[minSlackJob] == -1) {
				/*
				 * An augmenting path has been found.
				 */
				int committedJob = minSlackJob;
				int parentWorker = parentWorkerByCommittedJob[committedJob];
				while (true) {
					int temp = matchJobByWorker[parentWorker];
					match(parentWorker, committedJob);
					committedJob = temp;
					if (committedJob == -1) {
						break;
					}
					parentWorker = parentWorkerByCommittedJob[committedJob];
				}
				return;
			} else {
				/*
				 * Update slack values since we increased the size of the
				 * committed workers set.
				 */
				int worker = matchWorkerByJob[minSlackJob];
				committedWorkers[worker] = true;
				for (int j = 0; j < dim; j++) {
					if (parentWorkerByCommittedJob[j] == -1) {
						int slack = costMatrix[worker,j]
								- labelByWorker[worker] - labelByJob[j];
						if (minSlackValueByJob[j] > slack) {
							minSlackValueByJob[j] = slack;
							minSlackWorkerByJob[j] = worker;
						}
					}
				}
			}
		}
	}

	/**
	 * 
	 * @return the first unmatched worker or {@link #dim} if none.
	 */
	protected int fetchUnmatchedWorker() {
		int w;
		for (w = 0; w < dim; w++) {
			if (matchJobByWorker[w] == -1) {
				break;
			}
		}
		return w;
	}

	/**
	 * Find a valid matching by greedily selecting among zero-cost matchings.
	 * This is a heuristic to jump-start the augmentation algorithm.
	 */
	protected void greedyMatch() {
		for (int w = 0; w < dim; w++) {
			for (int j = 0; j < dim; j++) {
				if (matchJobByWorker[w] == -1
						&& matchWorkerByJob[j] == -1
						&& costMatrix[w,j] - labelByWorker[w] - labelByJob[j] == 0) {
					match(w, j);
				}
			}
		}
	}

	/**
	 * Initialize the next phase of the algorithm by clearing the committed
	 * workers and jobs sets and by initializing the slack arrays to the values
	 * corresponding to the specified root worker.
	 * 
	 * @param w
	 *            the worker at which to root the next phase.
	 */
	protected void initializePhase(int w) {
        committedWorkers.Fill(false);
        parentWorkerByCommittedJob.Fill(-1);
		committedWorkers[w] = true;
		for (int j = 0; j < dim; j++) {
			minSlackValueByJob[j] = costMatrix[w,j] - labelByWorker[w]
					- labelByJob[j];
			minSlackWorkerByJob[j] = w;
		}
	}

	/**
	 * Helper method to record a matching between worker w and job j.
	 */
	protected void match(int w, int j) {
		matchJobByWorker[w] = j;
		matchWorkerByJob[j] = w;
	}

	/**
	 * Reduce the cost matrix by subtracting the smallest element of each row
	 * from all elements of the row as well as the smallest element of each
	 * column from all elements of the column. Note that an optimal assignment
	 * for a reduced cost matrix is optimal for the original cost matrix.
	 */
	protected void reduce() {
		for (int w = 0; w < dim; w++) {
            int min1 = int.MaxValue;
			for (int j = 0; j < dim; j++) {
				if (costMatrix[w,j] < min1) {
					min1 = costMatrix[w,j];
				}
			}
			for (int j = 0; j < dim; j++) {
				costMatrix[w,j] -= min1;
			}
		}
		int[] min = new int[dim];
		for (int j = 0; j < dim; j++) {
            min[j] = int.MaxValue;
		}
		for (int w = 0; w < dim; w++) {
			for (int j = 0; j < dim; j++) {
				if (costMatrix[w,j] < min[j]) {
					min[j] = costMatrix[w,j];
				}
			}
		}
		for (int w = 0; w < dim; w++) {
			for (int j = 0; j < dim; j++) {
				costMatrix[w,j] -= min[j];
			}
		}
	}

	/**
	 * Update labels with the specified slack by adding the slack value for
	 * committed workers and by subtracting the slack value for committed jobs.
	 * In addition, update the minimum slack values appropriately.
	 */
	protected void updateLabeling(int slack) {
		for (int w = 0; w < dim; w++) {
			if (committedWorkers[w]) {
				labelByWorker[w] += slack;
			}
		}
		for (int j = 0; j < dim; j++) {
			if (parentWorkerByCommittedJob[j] != -1) {
				labelByJob[j] -= slack;
			} else {
				minSlackValueByJob[j] -= slack;
			}
		}
	}
}

public static class ArrayExtensions
{
    public static void Fill<T>(this T[] originalArray, T with)
    {
        for (int i = 0; i < originalArray.Length; i++)
        {
            originalArray[i] = with;
        }
    }
}