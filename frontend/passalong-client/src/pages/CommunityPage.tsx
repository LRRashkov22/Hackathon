import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { communityApi } from '../api/endpoints'
import type { ImpactSummary, LeaderboardEntry } from '../api/types'
import SiteHeader from '../components/SiteHeader'
import { errorMessage } from '../utils/format'
import '../styles/FeaturePages.css'
import '../styles/components.css'

export default function CommunityPage() {
  const [impact, setImpact] = useState<ImpactSummary | null>(null)
  const [leaders, setLeaders] = useState<LeaderboardEntry[]>([])
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([communityApi.impact(), communityApi.leaderboard()])
      .then(([summary, entries]) => {
        setImpact(summary)
        setLeaders(entries)
      })
      .catch((e) => setError(errorMessage(e)))
  }, [])

  return (
    <main className="feature">
      <SiteHeader />
      <section>
        <p className="eyebrow">COMMUNITY IMPACT</p>
        <h1>Small exchanges. Real local impact.</h1>
        {error && <p className="error-text">{error}</p>}

        <div className="metrics">
          {impact && (
            <>
              <div>
                <b>{impact.completedTransfers}</b>
                <span>completed transfers</span>
              </div>
              <div>
                <b>{impact.itemsReused}</b>
                <span>items reused</span>
              </div>
              <div>
                <b>{impact.activeListings}</b>
                <span>active listings</span>
              </div>
              <div>
                <b>{impact.members}</b>
                <span>community members</span>
              </div>
            </>
          )}
        </div>

        <div className="panel leaders">
          <h2>Community leaderboard</h2>
          {leaders.map((member, index) => (
            <p key={member.id}>
              <b>
                <Link to={`/users/${member.id}`}>
                  #{index + 1} {member.displayName}
                </Link>
              </b>
              <span>
                {member.reputationScore} reputation{member.location && ` · ${member.location}`}
              </span>
            </p>
          ))}
        </div>
      </section>
    </main>
  )
}
