import { useEffect, useMemo, useState } from "react";
import api from "../../services/api";

export default function LiveCompetitionSummary() {
  const [participants, setParticipants] = useState([]);
  const [loading, setLoading] = useState(true);

  const [search, setSearch] = useState("");
  const [role, setRole] = useState("All");
  const [limit, setLimit] = useState(100);

  const loadSummary = async () => {
    try {
      setLoading(true);

      const response = await api.get(
        "/reports/live-summary"
      );

      const data = response.data;

      if (data?.success) {
        setParticipants(data.participants || []);
      }
    } catch (error) {
      console.error(
        "Live summary loading failed:",
        error
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadSummary();

    // Live refresh
    const interval = setInterval(
      loadSummary,
      10000
    );

    return () => clearInterval(interval);
  }, []);


  const filteredParticipants = useMemo(() => {
    let data = [...participants];

    if (role !== "All") {
      data = data.filter(
        x => x.roleCategory === role
      );
    }

    if (search.trim()) {
      const q = search.toLowerCase();

      data = data.filter(x =>
        String(x.userId).includes(q) ||
        (x.userName || "")
          .toLowerCase()
          .includes(q) ||
        (x.schoolName || "")
          .toLowerCase()
          .includes(q) ||
        (x.cityName || "")
          .toLowerCase()
          .includes(q)
      );
    }

    data.sort(
      (a, b) =>
        Number(b.combinedScore || 0) -
        Number(a.combinedScore || 0)
    );

    return data.slice(0, limit);
  }, [
    participants,
    role,
    search,
    limit
  ]);


  const stats = useMemo(() => {

    const students =
      participants.filter(
        x => x.roleCategory === "Student"
      ).length;

    const teachers =
      participants.filter(
        x => x.roleCategory === "Teacher"
      ).length;

    const quizUsers =
      participants.filter(
        x => x.hasQuizActivity
      ).length;

    const numberUsers =
      participants.filter(
        x => x.hasNumberGameActivity
      ).length;

    const wordUsers =
      participants.filter(
        x => x.hasWordGameActivity
      ).length;

    return {
      total: participants.length,
      students,
      teachers,
      quizUsers,
      numberUsers,
      wordUsers
    };

  }, [participants]);


  return (
    <div className="container-fluid py-4">

      {/* HEADER */}

      <div className="d-flex justify-content-between align-items-center mb-4">

        <div>
          <h4 className="fw-bold mb-1">
            Live Competition Summary
          </h4>

          <div className="text-muted small">
            Real-time participant performance
          </div>
        </div>

        <div>
          <span className="badge bg-success-subtle text-success px-3 py-2 rounded-pill">
            <i className="bi bi-broadcast me-1"></i>
            LIVE
          </span>
        </div>

      </div>


      {/* STATS */}

      <div className="row g-3 mb-4">

        <StatCard
          title="Participants"
          value={stats.total}
          icon="bi-people-fill"
        />

        <StatCard
          title="Students"
          value={stats.students}
          icon="bi-mortarboard-fill"
        />

        <StatCard
          title="Teachers"
          value={stats.teachers}
          icon="bi-person-workspace"
        />

        <StatCard
          title="Quiz Players"
          value={stats.quizUsers}
          icon="bi-question-circle-fill"
        />

        <StatCard
          title="Number Game"
          value={stats.numberUsers}
          icon="bi-calculator-fill"
        />

        <StatCard
          title="Word Game"
          value={stats.wordUsers}
          icon="bi-fonts"
        />

      </div>


      {/* FILTER */}

      <div className="card border-0 shadow-sm rounded-4 mb-4">

        <div className="card-body">

          <div className="row g-3 align-items-center">

            <div className="col-md-5">

              <div className="input-group">

                <span className="input-group-text bg-white">
                  <i className="bi bi-search"></i>
                </span>

                <input
                  className="form-control"
                  placeholder="Search name, User ID, school..."
                  value={search}
                  onChange={e =>
                    setSearch(e.target.value)
                  }
                />

              </div>

            </div>


            <div className="col-md-3">

              <select
                className="form-select"
                value={role}
                onChange={e =>
                  setRole(e.target.value)
                }
              >
                <option value="All">
                  All Participants
                </option>

                <option value="Student">
                  Students
                </option>

                <option value="Teacher">
                  Teachers
                </option>
              </select>

            </div>


            <div className="col-md-4 text-md-end">

              <div className="btn-group">

                {[3, 10, 100].map(x => (

                  <button
                    key={x}
                    className={`btn ${
                      limit === x
                        ? "btn-dark"
                        : "btn-outline-dark"
                    }`}
                    onClick={() =>
                      setLimit(x)
                    }
                  >
                    Top {x}
                  </button>

                ))}

              </div>

            </div>

          </div>

        </div>

      </div>


      {/* TABLE */}

      <div className="card border-0 shadow-sm rounded-4">

        <div className="card-body p-0">

          <div className="table-responsive">

            <table className="table table-hover align-middle mb-0">

              <thead className="table-light">

                <tr>

                  <th className="px-4">
                    #
                  </th>

                  <th>
                    Participant
                  </th>

                  <th>
                    Role
                  </th>

                  <th>
                    Quiz
                  </th>

                  <th>
                    Number Game
                  </th>

                  <th>
                    Word Game
                  </th>

                  <th>
                    Games
                  </th>

                  <th>
                    Overall
                  </th>

                </tr>

              </thead>

              <tbody>

                {loading ? (

                  <tr>
                    <td
                      colSpan="8"
                      className="text-center py-5"
                    >
                      Loading live data...
                    </td>
                  </tr>

                ) : filteredParticipants.length === 0 ? (

                  <tr>
                    <td
                      colSpan="8"
                      className="text-center py-5 text-muted"
                    >
                      No participants found.
                    </td>
                  </tr>

                ) : (

                  filteredParticipants.map(
                    (user, index) => (

                      <tr key={user.userId}>

                        <td className="px-4 fw-bold">
                          {index + 1}
                        </td>

                        <td>

                          <div className="fw-semibold">
                            {user.userName || "Unknown"}
                          </div>

                          <div className="small text-muted">
                            ID: {user.userId}
                          </div>

                        </td>

                        <td>

                          <span className="badge bg-light text-dark border">
                            {user.roleCategory}
                          </span>

                        </td>

                        <td>
                          {user.quiz.totalMarksObtain}
                        </td>

                        <td>
                          {user.numberGame.totalScore}
                        </td>

                        <td>
                          {user.wordGame.totalScore}
                        </td>

                        <td>
                          <span className="badge bg-primary-subtle text-primary">
                            {user.gamesStarted}/3
                          </span>
                        </td>

                        <td>
                          <strong>
                            {user.combinedScore}
                          </strong>
                        </td>

                      </tr>

                    )
                  )

                )}

              </tbody>

            </table>

          </div>

        </div>

      </div>

    </div>
  );
}


function StatCard({
  title,
  value,
  icon
}) {
  return (
    <div className="col-12 col-md-6 col-lg-2">

      <div className="card border-0 shadow-sm rounded-4 h-100">

        <div className="card-body">

          <div className="text-muted small">
            {title}
          </div>

          <div className="d-flex justify-content-between align-items-center mt-2">

            <div className="fs-3 fw-bold">
              {value.toLocaleString("en-IN")}
            </div>

            <i className={`bi ${icon} fs-4 text-primary`}></i>

          </div>

        </div>

      </div>

    </div>
  );
}