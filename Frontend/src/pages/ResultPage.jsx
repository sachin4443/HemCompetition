import { useLocation, useNavigate } from "react-router-dom";

export default function ResultPage() {

    const navigate = useNavigate();
    const location = useLocation();

    const score = location.state?.score ?? 0;
    const total = location.state?.total ?? 0;

    return (

        <div className="container py-5">

            <div
                className="card shadow-lg border-0 rounded-4 mx-auto"
                style={{ maxWidth: 650 }}
            >

                <div className="card-body text-center p-5">

                    <div
                        className="bg-success text-white rounded-circle mx-auto mb-4 d-flex align-items-center justify-content-center"
                        style={{
                            width: 90,
                            height: 90,
                            fontSize: 40
                        }}
                    >
                        ✓
                    </div>

                    <h2 className="fw-bold">

                        Competition Completed

                    </h2>

                    <hr />

                    <div className="row mt-4">

                        <div className="col-md-6 mb-3">

                            <div className="alert alert-primary">

                                <h4>{score}</h4>

                                Score

                            </div>

                        </div>

                        <div className="col-md-6 mb-3">

                            <div className="alert alert-success">

                                <h4>{total}</h4>

                                Total Questions

                            </div>

                        </div>

                    </div>

                    <h4 className="mt-3">

                        Result

                    </h4>

                    <h1 className="text-success">

                        {Math.round((score / total) * 100)}%

                    </h1>

                    <button

                        className="btn btn-success btn-lg rounded-pill mt-4 px-5"

                        onClick={() => navigate("/dashboard")}

                    >

                        Back To Dashboard

                    </button>

                </div>

            </div>

        </div>

    );

}